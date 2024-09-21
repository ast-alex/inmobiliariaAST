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

        //ocultar perfiles de usuarios a empleados
        if(usuarioLogeado != null && usuarioLogeado.Rol == (int)Roles.Empleado && usuario.ID_usuario != usuarioLogeado.ID_usuario)
        {
            return RedirectToAction("AccessDenied" , "Auth");
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
            }else{
                usuario.Avatar = Usuario.AvatarDefault;
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
    //GETedicion
    public IActionResult Edicion(int id)
    {
        var usuario = repo.GetID(id);
        if (usuario == null)
        {
            return NotFound();
        }

        var email = User.Identity?.Name;
        Usuario? usuarioLogeado = null;

        if (!string.IsNullOrEmpty(email))
        {
            usuarioLogeado = repo.GetByEmail(email);
        }

        ViewBag.EsAdministrador = usuarioLogeado?.Rol == (int)Roles.Administrador;
        ViewBag.esPropioPerfil = usuarioLogeado?.Email == usuario.Email;

        return View(usuario);
    }
    
    //post
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edicion(int id, Usuario usuario, bool eliminarAvatar)
    {
        if (id != usuario.ID_usuario)
        {
            return NotFound();
        }

        if(eliminarAvatar){
            repo.EliminarAvatar(usuario.ID_usuario);
        }

        var usuarioActual = repo.GetID(id);
        if (usuarioActual == null)
        {
            return NotFound();
        }

        ModelState.Remove("Password");

        if (ModelState.IsValid)
        {
            // Manejo del Avatar
            if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
            {
                // Procesar la carga del nuevo avatar
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

                usuario.Avatar = "/uploads/avatars/" + uniqueFileName; // Actualiza el avatar
            }
            else
            {
                usuario.Avatar = usuarioActual.Avatar; // Mantiene el avatar actual si no se ha subido uno nuevo
            }

            // Mantener la contraseña actual
            usuario.Password = usuarioActual.Password;

            // Validación del estado
            var email = User.Identity?.Name;
            Usuario? usuarioLogeado = null;

            if (!string.IsNullOrEmpty(email))
            {
                usuarioLogeado = repo.GetByEmail(email);
            }

            if (usuarioLogeado != null)
            {
                // No permitir al usuario cambiar su propio rol
                if (usuarioLogeado.Email == usuario.Email)
                {
                    usuario.Rol = usuarioActual.Rol; // Mantener el rol actual si es su propio perfil
                }

                // Mantener el estado actual si no es administrador
                if (usuarioLogeado.Rol != (int)Roles.Administrador)
                {
                    usuario.Estado = usuarioActual.Estado;
                }
            }

            // Actualizar los demás campos
            repo.ActualizarUsuario(usuario);
            return RedirectToAction(nameof(Index));
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


    //verifico si usuario es admin
    private bool EsAdministrador()
    {
        var userId = int.Parse(User.FindFirst("ID_usuario")?.Value ?? "0");
        return repo.EsAdministrador(userId);
    }   

}
