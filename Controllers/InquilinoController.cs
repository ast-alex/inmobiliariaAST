using inmobiliariaAST.Models;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaAST.Controllers;

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
                return View(inquilino);
            }
        }

        [HttpPost]
        public IActionResult Guardar(int id, Inquilino inquilino)
        {
            id = inquilino.Id_inquilino;
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

        public IActionResult Eliminar(int id)
        {
            repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }

