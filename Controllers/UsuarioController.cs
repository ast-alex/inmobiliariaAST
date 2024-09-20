using Microsoft.AspNetCore.Mvc;
using inmobiliariaAST.Models;
using System.Security.Cryptography;
using inmobiliariaAST.Services;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class UsuarioController : Controller{
    
    private readonly ILogger<UsuarioController> _logger;
    private readonly RepositorioUsuario repo;
    private readonly AuthenticationService _authService;

    public UsuarioController(ILogger<UsuarioController> logger, AuthenticationService authService){
        _logger =logger;
        repo = new RepositorioUsuario();
        _authService = authService;
    }


    //get usuario
    public IActionResult Index()
    {
        var usuarios = repo.ObtenerTodosUsuariosIncluidosInactivos();
        return View(usuarios);
    }

    //password
    [HttpGet]
    public IActionResult Pass()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Pass(PasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Auth");
            }

            var usuarioLogeado = repo.GetByEmail(email);
            if (usuarioLogeado == null)
            {
                return NotFound();
            }

            // Verificar contraseña actual
            if (string.IsNullOrEmpty(usuarioLogeado.Password) || 
                string.IsNullOrEmpty(model.PasswordActual) || 
                !_authService.VerifyPassword(usuarioLogeado.Password, model.PasswordActual))
            {
                ModelState.AddModelError(string.Empty, "La contraseña actual es incorrecta");
                return View(model);
            }

            // Cambiar contraseña
            if (!string.IsNullOrEmpty(model.NuevaPassword))
            {
                usuarioLogeado.Password = _authService.HashPassword(model.NuevaPassword);
                repo.ActualizarUsuario(usuarioLogeado);
            }

            return RedirectToAction("Detalles", "Usuario", new { id = usuarioLogeado.ID_usuario });
        }

        return View(model);
    }


    //get usuario detalles
    public IActionResult Detalles(int id)
    {
      
        var usuario = repo.GetID(id);
        if(usuario ==null)
        {
            return NotFound();
        }

        var email = User.Identity?.Name;
        Usuario? usuarioLogeado = null;

        if (!string.IsNullOrEmpty(email))
        {
            usuarioLogeado = repo.GetByEmail(email);
        }

        ViewBag.EsAdministrador = usuarioLogeado != null && usuarioLogeado.Rol == (int)Roles.Administrador;

        return View(usuario);
    }

    //get crear
    public IActionResult Crear()
    {
        return View();
    }

    //post crear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Usuario usuario)
    {
        
        if(ModelState.IsValid)
        {
             // Procesar la subida del archivo
            if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
            {
                // Definir el directorio donde se almacenarán los avatares
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\avatars");

                // Asegurarse de que la carpeta exista
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generar un nombre de archivo único
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(usuario.AvatarFile.FileName);

                // Construir la ruta completa del archivo
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Guardar el archivo en el servidor
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    usuario.AvatarFile.CopyTo(fileStream);
                }

                // Guardar la ruta del archivo en la propiedad Avatar
                usuario.Avatar = "/uploads/avatars/" + uniqueFileName;
            }

            if (!string.IsNullOrEmpty(usuario.Password))
            {
                usuario.Password = _authService.HashPassword(usuario.Password);
            }
            //usuario.Password = _authService.HashPassword(usuario.Password);
            var id = repo.Alta(usuario);
            return RedirectToAction(nameof(Index));
        }
        return View(usuario);
    }

    //edicion
    public IActionResult Edicion(int id)
    {
        var usuario = repo.GetID(id);
        if(usuario==null)
        {
            return NotFound();
        }

        var email = User.Identity?.Name;
        Usuario? usuarioLogeado = null;

        if (!string.IsNullOrEmpty(email))
        {
            usuarioLogeado = repo.GetByEmail(email);
        }

        if (usuarioLogeado != null)
        {
            ViewBag.EsAdministrador = usuarioLogeado.Rol == (int)Roles.Administrador;
        }
        else
        {
            ViewBag.EsAdministrador = false;
        }

        return View(usuario);
    }

    //post
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edicion(int id, Usuario usuario)
    {
        
        if (id != usuario.ID_usuario)
        {
            return NotFound();
        }

        var email = User.Identity?.Name;
        Usuario? usuarioLogeado = null;

        if (!string.IsNullOrEmpty(email))
        {
            usuarioLogeado = repo.GetByEmail(email);
        }

        var usuarioActual = repo.GetID(id);

        if (usuarioActual == null)
        {
            return NotFound();
        }

        ModelState.Remove("Password");
        

        if (ModelState.IsValid)
        {
            // Si no es administrador, se impide modificar el rol
            if (usuarioLogeado != null && usuarioLogeado.Rol != (int)Roles.Administrador)
            {
                usuario.Rol = usuarioActual.Rol; // Se mantiene el rol actual si no es administrador
            }
            
            usuario.Password = usuarioActual.Password; // Mantener la contraseña actual

            // Procesar la subida del archivo (actualización del avatar)
            if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\avatars");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(usuario.AvatarFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    usuario.AvatarFile.CopyTo(fileStream);
                }

                usuario.Avatar = "/uploads/avatars/" + uniqueFileName;
            }
            else
            {
                usuario.Avatar = usuarioActual.Avatar; // Mantener el avatar actual si no se sube uno nuevo
            }
        
            // Actualizar los demás campos
            repo.ActualizarUsuario(usuario);
            return RedirectToAction(nameof(Index));
        }

        // Si hay un usuario logueado, pasar un indicador a la vista si es administrador
        if (usuarioLogeado != null)
        {
            ViewBag.EsAdministrador = usuarioLogeado.Rol == (int)Roles.Administrador;
        }
        else
        {
            ViewBag.EsAdministrador = false;
        }
            return View(usuario);
    }

    //eliminar
    [Authorize(Roles = "Administrador")]
    public IActionResult Eliminar(int id)
    {
        var usuario = repo.GetID(id);
        if(usuario== null)
        {
            return NotFound();
        }
        return View(usuario);
    }
    
    //post eliminar
    
    [HttpPost, ActionName("Eliminar")]
    [ValidateAntiForgeryToken]
    public IActionResult EliminarConfirmado(int id)
    {
        repo.EliminarUsuario(id);
        return RedirectToAction(nameof(Index));
    }

    //post eliminar
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador")]
    public IActionResult EliminarAvatar(int id)
    {
        repo.EliminarAvatar(id);
        return RedirectToAction(nameof(Detalles), new {id});
    }

    //verifico si usuario es admin
    private bool EsAdministrador()
    {
        var userId = int.Parse(User.FindFirst("ID_usuario")?.Value ?? "0");
        return repo.EsAdministrador(userId);
    }   

}
