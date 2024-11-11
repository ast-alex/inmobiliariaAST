using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliariaAST.Models;
using Microsoft.EntityFrameworkCore;
using Inmobiliaria.Models;


namespace inmobiliariaAST.Api{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContratoController : ControllerBase{

        private readonly IRepositorioContrato repoContrato;
        private readonly IRepositorioPropietario repositorioPropietario;
        private readonly DataContext _context;

        public ContratoController(IRepositorioContrato repoContrato, DataContext context){
            this.repoContrato = repoContrato;
            this.repositorioPropietario = new RepositorioPropietario();
            this._context = context;
        }

        //listar contratos
        [HttpGet("inmuebles-alquilados")]
        [Authorize]
        public IActionResult ListarInmueblesAlquilados(){
            try
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
                if(email == null) return Unauthorized("No se pudo obtener el email del propietario autenticado.");

                var propietario = _context.Propietario.FirstOrDefault(p => p.Email == email);
                if(propietario == null) return NotFound("No se encontró el propietario autenticado.");

                 // Obtener los contratos asociados al propietario a través del Inmueble
                var contrato = _context.Contrato
                    .Where(c => c.Inmueble.ID_propietario == propietario.ID_propietario && c.Estado)
                    .Include(c => c.Inmueble)  
                    .Include(c => c.Inquilino) 
                    .Select(c => new {
                        c.ID_contrato,
                        c.ID_inmueble,
                        InmuebleDireccion = c.Inmueble.Direccion,  
                        InmuebleFoto = c.Inmueble.Foto,
                    })
                    .ToList();
                return Ok(contrato);
            }catch(Exception ex){
                return StatusCode(500, new { Message = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        //detalle contrato
        [HttpGet("detalle/{idContrato}")]
        [Authorize]
        public IActionResult ObtenerDetalleContrato(int idContrato)
        {
            try
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
                if(email == null) return Unauthorized("No se pudo obtener el email del propietario autenticado.");

                var propietario = _context.Propietario.FirstOrDefault(p => p.Email == email);
                if(propietario == null) return NotFound("No se encontró el propietario autenticado.");

                var contrato = _context.Contrato
                    .Where(c => c.ID_contrato == idContrato && c.Inmueble.ID_propietario == propietario.ID_propietario)
                    .Include(c => c.Inmueble)
                    .Include(c => c.Inquilino)
                    .FirstOrDefault();

                if(contrato == null) return NotFound("No se encontró el contrato.");

                var detalleContrato = new {
                    contrato.ID_contrato,
                    contrato.ID_inmueble,
                    Fecha_Inicio = contrato.Fecha_Inicio.ToString("dd/MM/yyyy"),
                    Fecha_Fin = contrato.Fecha_Fin.ToString("dd/MM/yyyy"),
                    contrato.Monto_Mensual,
                    InmuebleDireccion = contrato.Inmueble.Direccion,
                    InmuebleFoto = contrato.Inmueble.Foto,
                    InquilinoNombreCompleto = contrato.Inquilino.Nombre + " " + contrato.Inquilino.Apellido
                };

                return Ok(detalleContrato);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

