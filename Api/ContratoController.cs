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

        public ContratoController(IRepositorioContrato repoContrato){
            this.repoContrato = repoContrato;
            this.repositorioPropietario = new RepositorioPropietario();
        }

        //listar contratos
        [HttpGet("inmuebles-alquilados")]
        [Authorize]
        public IActionResult ListarInmueblesAlquilados(){
            try
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
                if(email == null) return Unauthorized("No se pudo obtener el email del propietario autenticado.");

                var propietario = repositorioPropietario.GetByEmail(email);
                if(propietario == null) return NotFound("No se encontró el propietario autenticado.");

                var contratos = repoContrato.ListarContratosPorPropietario(propietario.ID_propietario);

                var inmueblesAlquilados = contratos.Select(c=>new{
                    c.ID_inmueble,
                    c.InmuebleDireccion,
                    c.InmuebleFoto,
                    c.Fecha_Inicio,
                    c.Fecha_Fin,
                    c.Monto_Mensual,
                    c.InquilinoNombreCompleto,
                    c.Fecha_Terminacion_Anticipada,
                    c.Multa
                }).ToList();

                return Ok(inmueblesAlquilados);
            }catch(Exception ex){
                return StatusCode(500, new { Message = ex.Message, StackTrace = ex.StackTrace });
            }
        }

        //detalle contrato
        [HttpGet("detalle/{idInmueble}")]
        [Authorize]
        public IActionResult ObtenerDetalleContrato(int idInmueble)
        {
            try
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
                if(email == null) return Unauthorized("No se pudo obtener el email del propietario autenticado.");

                var propietario = repositorioPropietario.GetByEmail(email);
                if(propietario == null) return NotFound("No se encontró el propietario autenticado.");

                var contrato = repoContrato.GetDetalle(idInmueble, propietario.ID_propietario);
                if(contrato == null) return NotFound("No se encontró un contrato activo para este inmueble.");

                var detalleContrato = new {
                    contrato.ID_inmueble,
                    contrato.InmuebleDireccion,
                    contrato.InmuebleFoto,
                    contrato.Fecha_Inicio,
                    contrato.Fecha_Fin,
                    contrato.Monto_Mensual,
                    contrato.InquilinoNombreCompleto,
                    contrato.Fecha_Terminacion_Anticipada,
                    contrato.Multa
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
