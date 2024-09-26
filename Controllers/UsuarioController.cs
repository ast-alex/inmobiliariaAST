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
    [Authorize]
    public IActionResult Index()
    {
        // Obtener el usuario autenticado
        var usuarioAutenticado = User.Identity?.Name;
        if (string.IsNullOrEmpty(usuarioAutenticado))
        {
            //en caso de que el usuario autenticado sea nulo redireccionado al login 
            return RedirectToAction("Login", "Auth");
        }

        // Si es Administrador, mostramos todos los usuarios
        if (User.IsInRole("Administrador"))
        {
            var usuarios = repo.ObtenerTodosUsuariosIncluidosInactivos();
            return View(usuarios);
        }
        else
        {
            // Si no es administrador, mostramos solo su perfil
            var usuario = repo.GetByEmail(usuarioAutenticado);
            return View(new List<Usuario?> { usuario });
        }
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
                var nuevoHash = _authService.HashPassword(model.NuevaPassword);
                usuarioLogeado.Password = nuevoHash;

                // Actualizar el usuario en la base de datos
                repo.ActualizarUsuario(usuarioLogeado);
            }
            // return RedirectToAction("Detalles", "Usuario", new { id = usuarioLogeado.ID_usuario });
            return RedirectToAction("Index", "Home");
        }
        return View(model);
    }


    //get usuario detalles
    // [Authorize(Roles = "Administrador")]
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
            TempData["SuccessMessage"] = "Usuario creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        return View(usuario);
    }
    
    
    //GETedicion
    [HttpGet]
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

        // Permitir a los empleados editar solo su propio perfil
        if (usuarioLogeado != null && usuarioLogeado.Rol == (int)Roles.Empleado && usuarioLogeado.ID_usuario != usuario.ID_usuario)
        {
            return RedirectToAction("AccessDenied", "Auth");
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

        // Verifica si el usuario logueado es un empleado y está intentando editar su perfil
        var email = User.Identity?.Name;
        Usuario? usuarioLogeado = null;

        if (!string.IsNullOrEmpty(email))
        {
            usuarioLogeado = repo.GetByEmail(email);
        }

        // Permitir a los empleados editar solo su propio perfil
        if (usuarioLogeado != null && usuarioLogeado.Rol == (int)Roles.Empleado && usuarioLogeado.ID_usuario != usuario.ID_usuario)
        {
            return RedirectToAction("AccessDenied", "Auth");
        }

        if (eliminarAvatar)
        {
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
            if (usuarioLogeado != null && usuarioLogeado.Rol != (int)Roles.Administrador)
            {
                usuario.Estado = usuarioActual.Estado; // Mantener el estado actual si no es administrador
            }

            // Actualizar los demás campos
            repo.ActualizarUsuario(usuario);
            TempData["SuccessMessage"] = "Cambios guardados exitosamente";
            
            //redirigir segun rol: 
            if(usuarioLogeado?.Rol == (int)Roles.Administrador){
                return RedirectToAction(nameof(Index));
            }else{

                return RedirectToAction("Detalles", new { id = usuarioLogeado?.ID_usuario });
            }
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
