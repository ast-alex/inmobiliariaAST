using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliariaAST.Models;
using Microsoft.EntityFrameworkCore;
using inmobiliariaAST.Services;

namespace inmobiliariaAST.Api
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PagoController : ControllerBase{
        private readonly DataContext _context;

        public PagoController(DataContext context)
        {
            _context = context;
        }

        //listar PAGOS de un contrato específico ACTIVOS E INACTIVOS
        [HttpGet("pagos-contrato/{idContrato}")]
        [Authorize]
        public IActionResult getPagosPorContrato(int idContrato)
        {
            try{
                var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
                if(email == null) return Unauthorized("No se pudo obtener el email del propietario autenticado.");

                //obtener pagos asociados al contrato especificado
                var pagos = _context.Pago
                    .Where(p => p.ID_contrato == idContrato)
                    .Select(p => new{
                        p.ID_pago,
                        p.ID_contrato,
                        p.Numero_pago,
                        Fecha_pago = p.Fecha_pago.ToString("yyyy-MM-dd"),
                        p.Importe,
                        p.Estado,
                        p.Concepto,
                    })
                    .ToList();

                if(pagos == null || !pagos.Any()){
                    return NotFound("No se encontraron pagos asociados al contrato.");
                }

                return Ok(pagos);

            }catch(Exception ex){
                return StatusCode(500, new { Message = ex.Message, StackTrace = ex.StackTrace });
            }
        }


    }
}