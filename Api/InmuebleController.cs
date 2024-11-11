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
using Inmobiliaria.Models;


namespace inmobiliariaAST.Api{


    [Route("api/[controller]")]
    [ApiController]
    [Authorize]    
    public class InmuebleController : ControllerBase
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioPropietario repositorioPropietario;
        private readonly DataContext _context;
        public InmuebleController(IRepositorioInmueble repositorio, IRepositorioPropietario repositorioPropietario, DataContext context)
        {       
            this.repositorio = repositorio;
            this.repositorioPropietario = repositorioPropietario;
            this._context = context;
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
            var inmuebles = _context.Inmueble
                .Where(i => i.ID_propietario == idPropietario)
                .Select(i => new{
                    i.ID_inmueble,
                    i.Direccion,
                    i.Precio,
                    i.Foto,
                    i.ID_propietario
                }).ToList();
                
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
            var propietario = _context.Propietario.FirstOrDefault(p => p.Email == email);
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
            _context.Inmueble.Add(inmueble);
            _context.SaveChanges();

            return Ok(new { ID_inmueble = inmueble.ID_inmueble, inmueble });

            }catch(Exception ex){
                return StatusCode(500, ex.Message);
            }
        }


        //detalle inmueble especifico
        [HttpGet("detalle/{id}")]
        [Authorize]
        public IActionResult DetalleInmueble(int id)
        {
            try
            {
                //obtener el email del propietario autenticado desde el token
                var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
                if (email == null) return Unauthorized("No se pudo obtener el email del propietario autenticado.");

                //verificar si el propietario autenticado es el propietario del inmueble
                var propietario = _context.Propietario.FirstOrDefault(p => p.Email == email);
                if (propietario == null) return NotFound("No se encontró el propietario autenticado.");

                //se obtiene el detalle del inmueble
                var inmueble = _context.Inmueble.FirstOrDefault(i => i.ID_inmueble == id);
                if(inmueble == null){
                    return NotFound("No se encontró el inmueble.");
                }

                //verificar si el inmueble pertenece al propietario autenticado
                if(inmueble.ID_propietario != propietario.ID_propietario){
                    return BadRequest("El inmueble no le pertenece al propietario autenticado.");
                }
                
                //vm detalle
                var inmuebleVM = new InmuebleDetalleViewModel
                {
                    ID_inmueble = inmueble.ID_inmueble,
                    Direccion = inmueble.Direccion,
                    Uso = inmueble.Uso.ToString(),
                    Tipo = inmueble.Tipo.ToString(),
                    Cantidad_Ambientes = inmueble.Cantidad_Ambientes,
                    Latitud = inmueble.Latitud,
                    Longitud = inmueble.Longitud,
                    Precio = inmueble.Precio,
                    Disponibilidad = inmueble.Disponibilidad,
                    Foto = inmueble.Foto
                };

                return Ok(inmuebleVM);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //cambiar disponibilidad PATCH
        [HttpPatch("disponibilidad/{id}")]
        [Authorize]
        public IActionResult ActualizarDisponibilidad (int id, [FromBody] bool disponibilidad)
        {
            try{
                var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
                if (email == null) return Unauthorized("No se pudo obtener el email del propietario autenticado.");

                var propietario = _context.Propietario.FirstOrDefault(p => p.Email == email);
                if (propietario == null) return NotFound("No se encontró el propietario autenticado.");

                var inmueble = _context.Inmueble.FirstOrDefault(i => i.ID_inmueble == id);
                if (inmueble == null) return NotFound("No se encontró el inmueble.");

                if(inmueble.ID_propietario != propietario.ID_propietario){
                    return BadRequest("El inmueble no le pertenece al propietario autenticado.");
                }
                
                //actualizar disponibilidad
                inmueble.Disponibilidad = disponibilidad;
                _context.SaveChanges();
                
                return Ok("Disponibilidad cambiada exitosamente a: " + disponibilidad);

            }catch(Exception ex){
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("alquilados")]
        [Authorize]
        public IActionResult ListarInmueblesAlquilados()
        {
            try
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
                if (email == null) return Unauthorized("No se pudo obtener el email del propietario autenticado.");

                var propietario = _context.Propietario.FirstOrDefault(p => p.Email == email);       
                if (propietario == null) return NotFound("No se encontró el propietario autenticado.");

                var inmueblesAlquilados = _context.Inmueble
                    .Where(i => i.ID_propietario == propietario.ID_propietario && 
                                _context.Contrato.Any(c => c.ID_inmueble == i.ID_inmueble && c.Estado == true)) 
                    .Select(i => new
                    {
                        i.ID_inmueble,
                        i.Direccion,
                        i.Foto
                    })
                    .ToList();

                return Ok(inmueblesAlquilados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}