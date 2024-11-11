using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliariaAST.Models;
using Microsoft.EntityFrameworkCore;
using inmobiliariaAST.Services;

namespace inmobiliariaAST.Api{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InquilinoController : ControllerBase{

        private readonly DataContext _context;
        private readonly IRepositorioInquilino repo;
        private readonly IRepositorioContrato repositorioContrato;

        public InquilinoController(IRepositorioInquilino repositorio, DataContext context, IRepositorioContrato repositorioContrato){
            this.repo = repositorio;
            this.repositorioContrato = repositorioContrato;
            this._context = context;

        }

        //obtener inquilino asociado al inmueble
        [HttpGet("{inmuebleId}")]
        [Authorize]
        public IActionResult getInquilino(int inmuebleId){
            try{
                var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
                if (email == null) return Unauthorized("No se pudo obtener el email del propietario autenticado.");

                var propietario = _context.Propietario.FirstOrDefault(p => p.Email == email);
                if (propietario == null) return NotFound("No se encontró el propietario autenticado.");

                // Obtener el contrato que corresponde al inmueble y al propietario autenticado
                var contrato = _context.Contrato
                    .Include(c => c.Inmueble)
                    .FirstOrDefault(c => c.ID_inmueble == inmuebleId && c.Inmueble.ID_propietario == propietario.ID_propietario);
                
                if (contrato == null) return NotFound("No se encontró el contrato del inmueble.");

                // Obtener el inquilino asociado al contrato
                var inquilino = _context.Inquilino
                    .FirstOrDefault(i => i.ID_inquilino == contrato.ID_inquilino);
                
                if (inquilino == null) return NotFound("No se encontró el inquilino.");

                // Obtener la dirección y foto del inmueble desde la entidad Inmueble relacionada
                var inmuebleDireccion = contrato.Inmueble?.Direccion;
                var inmuebleFoto = contrato.Inmueble?.Foto;

                // Devolver el inquilino junto con la dirección del inmueble
                return Ok(new 
                {
                    Inquilino = inquilino,
                    InmuebleDireccion = inmuebleDireccion,
                    InmuebleFoto = inmuebleFoto
                });
            } catch(Exception ex) {
                return StatusCode(500, ex.Message);
            }
}



    }


}