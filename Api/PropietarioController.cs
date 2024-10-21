using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using inmobiliariaAST.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using inmobiliariaAST.Services;

namespace inmobiliariaAST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropietarioController : ControllerBase
    {
        private readonly IRepositorioPropietario _repositorioPropietario;
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        private readonly AuthenticationService _authService;

        public PropietarioController(IRepositorioPropietario repositorioPropietario, IConfiguration config, AuthenticationService authService, DataContext context)
        {
            _repositorioPropietario = repositorioPropietario;
            _config = config;
            _authService = authService;
            _context = context;
        }

        // GET: api/Propietario/test
        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            try
            {
                return Ok("anduvo");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Propietarios/login
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginViewModel loginView)
        {
            if (loginView == null || string.IsNullOrEmpty(loginView.Email) || string.IsNullOrEmpty(loginView.Password))
            {
                return BadRequest("Email y Password son requeridos");
            }

            try
            {
                // Registro de valores recibidos
                Console.WriteLine($"Email: {loginView.Email}, Password: {loginView.Password}");
                // Buscar el propietario en la base de datos por email
                var propietario = _context.Propietario.FirstOrDefault(x => x.Email == loginView.Email); // Verifica que GetByEmail devuelva el propietario correctamente

                if (propietario == null)
                {
                    return BadRequest("Nombre de usuario o clave incorrecta"); // Usuario no encontrado
                }

                Console.WriteLine($"Contraseña hasheada en base de datos: {propietario.Password}");

                // Verificar la contraseña utilizando el servicio de autenticación
                if (!_authService.VerifyPassword(propietario.Password, loginView.Password))
                {
                    return BadRequest("Nombre de usuario o clave incorrecta"); // Contraseña incorrecta
                }

                // Generar el token si las credenciales son correctas
                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["TokenAuthentication:SecretKey"]));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, propietario.Email),
                    new Claim("FullName", $"{propietario.Nombre} {propietario.Apellido}"),
                    new Claim(ClaimTypes.Role, "Propietario"),
                };

                var token = new JwtSecurityToken(
                    issuer: _config["TokenAuthentication:Issuer"],
                    audience: _config["TokenAuthentication:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(35),
                    signingCredentials: credenciales
                );

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("perfil")]
		public async Task<ActionResult<Propietario>> Get()
        {
            try
            {
                // Obtener el email del token
                var usuario = User.Identity?.Name;

                // Verificar si el usuario fue extraído correctamente del token
                if (string.IsNullOrEmpty(usuario))
                {
                    return Unauthorized("No se pudo obtener la información del usuario.");
                }

                // Buscar al propietario en la base de datos por su email
                var propietario = await _context.Propietario.SingleOrDefaultAsync(x => x.Email == usuario);

                // Verificar si se encontró el propietario
                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado.");
                }

                // Retornar el propietario si todo está correcto
                return Ok(new 
                {
                    propietario.DNI,
                    propietario.Nombre,
                    propietario.Apellido,
                    propietario.Telefono,
                    propietario.Email,
                    propietario.Direccion,
                    Avatar = propietario.Avatar ?? Propietario.AvatarDefault
                });
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción
                return BadRequest($"Ocurrió un error: {ex.Message}");
            }
        }
        

        ///modificar perfil
        [HttpPut("modificar")]
        public async Task<IActionResult> Modificar([FromForm] PerfilViewModel model){
            try{
                var usuario = User.Identity?.Name;

                if(string.IsNullOrEmpty(usuario)){
                    return Unauthorized("No se pudo obtener la información del usuario.");
                }

                var propietario = await _context.Propietario.SingleOrDefaultAsync(x => x.Email == usuario);

                if(propietario == null){
                    return NotFound("Propietario no encontrado.");
                }

                //actualizar datos del propietario
                propietario.DNI = model.DNI ?? propietario.DNI;
                propietario.Nombre = model.Nombre ?? propietario.Nombre;
                propietario.Apellido = model.Apellido ?? propietario.Apellido;
                propietario.Telefono = model.Telefono ?? propietario.Telefono;
                propietario.Email = model.Email ?? propietario.Email;
                propietario.Direccion = model.Direccion ?? propietario.Direccion;

                //manejo del avatar
                if(model.AvatarFile != null && model.AvatarFile.Length > 0){
                    //lugar donde se guardan los avatar
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\avatars");

                    if(!Directory.Exists(uploadsFolder)){
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    //generar un nombre de archivo unico
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.AvatarFile.FileName);

                    //crear la ruta completa del archivo
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    //guardar el archivo en el server
                    using(var fileStream = new FileStream(filePath, FileMode.Create)){
                        model.AvatarFile.CopyTo(fileStream);
                    }

                    propietario.Avatar = "/uploads/avatars/" + uniqueFileName;

                }else{
                    propietario.Avatar = Propietario.AvatarDefault;
                }

                //guardar cambios en la base de datos
                _context.Entry(propietario).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new{
                    propietario.DNI,
                    propietario.Nombre,
                    propietario.Apellido,
                    propietario.Telefono,
                    propietario.Email,
                    propietario.Direccion,
                    propietario.Avatar
                });
            }
            catch(Exception ex){
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }
        }

        //cambiar password utilizando PasswordViewModel
        [HttpPut("cambiar-password")]
        public async Task<IActionResult> CambiarPassword([FromBody] PasswordViewModel model)
        {
            try
            {
                var usuario = User.Identity?.Name;

                if (string.IsNullOrEmpty(usuario)){
                    return Unauthorized("No se pudo obtener la información del usuario.");
                }

                var propietario = await _context.Propietario.SingleOrDefaultAsync(x => x.Email == usuario);

                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado.");

                }

                // Verificar la contraseña utilizando el servicio de autenticación
                if (!_authService.VerifyPassword(propietario.Password, model.PasswordActual))
                {
                    ModelState.AddModelError(string.Empty, "La contraseña actual es incorrecta.");
                    return BadRequest(ModelState);
                }

                // Cambiar contraseña
                propietario.Password = _authService.HashPassword(model.NuevaPassword);
                _context.Entry(propietario).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }
        }

    }
}
