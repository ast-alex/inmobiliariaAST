using inmobiliariaAST.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliariaAST.Controllers;

    [Authorize]
    public class InquilinoController : Controller
    {
        private readonly ILogger<InquilinoController> _logger;
        private RepositorioInquilino repo;

        public InquilinoController(ILogger<InquilinoController> logger)
        {
            _logger = logger;
            repo = new RepositorioInquilino();
        }

        public IActionResult Index()
        {
            var lista = repo.GetInquilinos();
            return View(lista);
        }
     
        public IActionResult Edicion(int id)
        {
            if (id == 0)
                return View();
            else
            {
                var inquilino = repo.Get(id);
                TempData["SuccessMessage"] = "Cambios guardados exitosamente";
                return View(inquilino);
            }
        }

        [HttpPost]
        public IActionResult Guardar(int id, Inquilino inquilino)
        {
            id = inquilino.ID_inquilino;
            if (id == 0)
            {
                repo.Alta(inquilino);
            }
            else
            {
                repo.Modificar(inquilino);
            }

            return RedirectToAction(nameof(Index));
        }

        //detalles inquilino
        public IActionResult Detalles(int id){

            var inquilino = repo.Get(id);
            if(inquilino == null){
                return NotFound();
            }
            return View(inquilino);
        }


        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {   
            var propietario = repo.Get(id);
            if(propietario == null){
                return NotFound();
            }

            repo.Baja(id);
            TempData["SuccessMessage"] = "El propietario ha sido dado de baja con sus activos relacionados.";
            return RedirectToAction(nameof(Index));
        }
    }

