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
        [HttpGet("inquilino/{inmuebleId}")]
        [Authorize]
        public IActionResult getInquilino(int inmuebleId){
            
            try{
                var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
                if (email == null) return Unauthorized("No se pudo obtener el email del propietario autenticado.");

                var propietario = _context.Propietario.FirstOrDefault(p => p.Email == email);
                if (propietario == null) return NotFound("No se encontró el propietario autenticado.");

                var inquilino = repositorioContrato.GetInquilinoPorInmueble(inmuebleId, propietario.ID_propietario);
                if (inquilino == null) return NotFound("No se encontró el inquilino.");

                return Ok(inquilino);
            }catch(Exception ex){
                return StatusCode(500, ex.Message);
            }
        }


    }


}