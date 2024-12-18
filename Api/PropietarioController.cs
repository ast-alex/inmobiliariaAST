using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using inmobiliariaAST.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
//smtp
using MimeKit;
using MailKit.Net.Smtp;



using Microsoft.EntityFrameworkCore;
using inmobiliariaAST.Services;
using MailKit.Security;


namespace inmobiliariaAST.Api
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
        [Authorize]
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

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> resetPassword([FromQuery] string email, [FromQuery] string token, [FromBody] ResetPasswordViewModel model){
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Todos los campos son obligatorios."); 

                //verificar si el token es valido
                var propietario = await _context.Propietario.SingleOrDefaultAsync(x => x.Email == email);

                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado.");
                }

                // //verificacion contraseña es correcta
                // if (!_authService.VerifyPassword(propietario.Password, model.PasswordActual)){
                //     return Unauthorized("La contraseña es incorrecta.");
                // }

                //hash
                var hashedPassword = _authService.HashPassword(model.NuevaPassword);
                propietario.Password = hashedPassword;

                _context.Entry(propietario).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                
                return Ok("Contraseña actualizada correctamente.");
            
            }catch(Exception ex){
                return BadRequest($"Ocurrio un error: {ex.Message}");
            }
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromForm] string email)
        {
            try
            {
                // Buscar el propietario por el correo electrónico
                var propietario = await _context.Propietario.FirstOrDefaultAsync(x => x.Email == email);

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("El email es requerido.");
                }

                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado.");
                }

                // Generar token único
                var token = Guid.NewGuid().ToString();

                // Establecer token y su fecha de expiración
                propietario.ResetToken = token;
                propietario.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token válido por 1 hora
                await _context.SaveChangesAsync();

                // Generar el enlace de restablecimiento
                var resetLink = $"{Request.Scheme}://{Request.Host}/api/Propietario/{propietario.ID_propietario}/change-password?token={token}";

                // Enviar el correo con el enlace de restablecimiento
                await EnviarCorreoAsync(email, "Restablecimiento de Contraseña", $"Haga clic en el siguiente enlace para restablecer su contraseña: {resetLink}");

                return Ok("Se ha enviado un enlace de restablecimiento de contraseña a su correo electrónico.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error: {ex.Message}");
            }
        }

        private async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Inmobiliaria AST", _config["SMTP_User"]));
            mensaje.To.Add(new MailboxAddress(destinatario, destinatario));
            mensaje.Subject = asunto;
            mensaje.Body = new TextPart("plain") { Text = cuerpo };

            using var cliente = new SmtpClient();
            if(int.TryParse(_config["SMTP_Port"], out int smtpport)){
                await cliente.ConnectAsync(_config["SMTP_Host"], smtpport, true);
            }else{
                throw new ArgumentException("El puerto SMTP debe ser un entero.");
            }

            await cliente.AuthenticateAsync(_config["SMTP_User"], _config["SMTP_Pass"]);
            await cliente.SendAsync(mensaje);
            await cliente.DisconnectAsync(true);
        }

        

        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> RestablecerContraseña(int id, [FromBody] RestablecerContrasenaRequest request)
        {
            try
            {
                // Buscar al propietario por su ID
                var propietario = await _context.Propietario.FindAsync(id);
                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado.");
                }

                // Verificar el token de restablecimiento
                if (propietario.ResetToken != request.Token || propietario.ResetTokenExpiry < DateTime.UtcNow)
                {
                    return BadRequest("Token de restablecimiento inválido o expirado.");
                }

                // Actualizar la contraseña
                propietario.Password = _authService.HashPassword(request.NuevaContrasena); // Asegúrate de usar un método para hashear la contraseña
                //ropietario.ResetToken = null; // Limpiar el token de restablecimiento
                //propietario.ResetTokenExpiry = null; // Limpiar la fecha de expiración del token

                await _context.SaveChangesAsync();

                return Ok("Contraseña restablecida con éxito.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocurrió un error: {ex.Message}");
            }
        }

            
    }
}

public class RestablecerContrasenaRequest
{
    public string? Token { get; set; }
    public string? NuevaContrasena { get; set; }
}
