using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using inmobiliariaAST.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


using Microsoft.EntityFrameworkCore;
using inmobiliariaAST.Services;


namespace inmobiliariaAST.Api{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]    
    public class InmuebleController : ControllerBase
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioPropietario repositorioPropietario;
        public InmuebleController(IRepositorioInmueble repositorio, IRepositorioPropietario repositorioPropietario)
        {       
            this.repositorio = repositorio;
            this.repositorioPropietario = repositorioPropietario;
        }

        //obtener inmuebles del propietario autenticado
        [HttpGet("inmueblespropietario")]
        [Authorize]
        public IActionResult GetInmueblesPorPropietario(){
            // Obtener el email del propietario autenticado desde el token
            var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

            if (email == null)
            {
                return Unauthorized("No se pudo obtener el email del propietario autenticado.");
            }

            // Obtener el ID del propietario a partir del email 
            var propietario = repositorioPropietario.GetByEmail(email);
            if (propietario == null)
            {
                return NotFound("No se encontró el propietario autenticado.");
            }
            Console.WriteLine($"Propietario encontrado: {propietario.Nombre} {propietario.Apellido}, ID: {propietario.ID_propietario}");
            int idPropietario = propietario.ID_propietario;

            //obtener los inmuebles del propietario autenticado
            Console.WriteLine($"ID del propietario para consultar inmuebles: {idPropietario}");
            var inmuebles = repositorio.GetInmueblesPorPropietario(idPropietario);
            Console.WriteLine($"Inmuebles encontrados: {inmuebles.Count}");

            if(inmuebles == null || inmuebles.Count == 0){
                return NotFound("No se encontraron inmuebles para el propietario autenticado.");
            }

            return Ok(inmuebles);
        }
        
       
        [HttpPost("crearInmueble")]
        [Authorize]
        public IActionResult CrearInmueble([FromForm] Inmueble inmueble)
        {
            try{
            // Obtener el email del propietario autenticado desde el token
            var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;

            if (email == null)
            {
                return Unauthorized("No se pudo obtener el email del propietario autenticado.");
            }

            // Obtener el ID del propietario a partir del email
            var propietario = repositorioPropietario.GetByEmail(email);
            if (propietario == null)
            {
                return NotFound("No se encontró el propietario autenticado.");
            }

            // Asignar el ID del propietario autenticado al inmueble
            inmueble.ID_propietario = propietario.ID_propietario;

            inmueble.Disponibilidad = false;

            Console.WriteLine(inmueble.FotoFile?.FileName ?? "FotoFile es null"); // Debug: Verificar si FotoFile llega correctamente

            if (inmueble.FotoFile == null)
            {
                return BadRequest("FotoFile no se recibió correctamente.");
            }

            // Guardar la imagen de la foto, si fue enviada
            if (inmueble.FotoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "fotos");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(inmueble.FotoFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    inmueble.FotoFile.CopyTo(fileStream);
                }

                inmueble.Foto = "/uploads/fotos/" + uniqueFileName;
                Console.WriteLine($"Foto guardada en: {inmueble.Foto}"); // Debug: Verificar la ruta guardada
            }

            // Guardar el inmueble en la base de datos
            var idInmueble = repositorio.Alta(inmueble);

            if (idInmueble > 0)
            {
                return Ok(new { ID_inmueble = idInmueble, inmueble });
            }

            return BadRequest("No se pudo crear el inmueble.");
            }catch(Exception ex){
                return StatusCode(500, ex.Message);
            }
        }



    }
}