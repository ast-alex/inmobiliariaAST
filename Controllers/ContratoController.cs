using inmobiliariaAST.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace inmobiliariaAST.Controllers{

    public class ContratoController : Controller
    {
        private readonly ILogger<ContratoController> _logger;
        private readonly RepositorioContrato repoContrato;
        private readonly RepositorioInmueble repoInmueble;
        private readonly RepositorioInquilino repoInquilino;

        public ContratoController(ILogger<ContratoController> logger)
        {
            _logger = logger;
            repoContrato = new RepositorioContrato();
            repoInmueble = new RepositorioInmueble();
            repoInquilino = new RepositorioInquilino();
        }

        //listar contratos
        public IActionResult Index()
        {
            var lista = repoContrato.GetContratos();
            return View(lista);
        }

        //mostrar form de crear contraro
        public IActionResult Crear()
        {
            ViewBag.Inmueble = new SelectList(
                repoInmueble.GetInmuebles(), //metodo para obtener la lista de inmuebles
                "ID_inmueble", 
                "Direccion"
            );

            ViewBag.Inquilino = new SelectList(
                repoInquilino.GetInquilinos(), //metodo para obtener la lista de inquilinos
                "ID_inquilino", 
                "Nombre",
                "Apellido"
            );

            return View();
        }

        //envio del form de crear contrato
        [HttpPost]
        public IActionResult Crear(Contrato contrato)
        {
            if(ModelState.IsValid)
            {
                repoContrato.Alta(contrato);
                return RedirectToAction(nameof(Index));
            }

            //si no es valido, volvemos a mostrar el form
            ViewBag.Inmueble = new SelectList(
                repoInmueble.GetInmuebles(), //metodo para obtener la lista de inmuebles
                "ID_inmueble", 
                "Direccion"
            );

            ViewBag.Inquilino = new SelectList(
                repoInquilino.GetInquilinos(), //metodo para obtener la lista de inquilinos
                "ID_inquilino", 
                "Nombre",
                "Apellido"
            );

            return View(contrato);
        }


        //modificar
        [HttpGet]
        public IActionResult Edicion(int id)
        {
            var contrato = repoContrato.Get(id);
            if(contrato == null)
            {
                return NotFound();
            }

            //recuperar la lista de inmuebles e inquilinos 
            ViewBag.Inmueble = new SelectList(
                repoInmueble.GetInmuebles(), //metodo para obtener la lista de inmuebles
                "ID_inmueble", 
                "Direccion",
                contrato.ID_inmueble
            );

            ViewBag.Inquilino = new SelectList(
                repoInquilino.GetInquilinos(), //metodo para obtener la lista de inquilinos
                "ID_inquilino", 
                "Nombre",
                contrato.ID_inquilino
            );

            return View(contrato);
        }

        //envio del form de modificar contrato
        [HttpPost]
        public IActionResult Edicion(Contrato contrato)
        {
            if(ModelState.IsValid)
            {
                repoContrato.Modificar(contrato);
                return RedirectToAction(nameof(Index));
            }

            //si no es valido, volvemos a mostrar el form
            ViewBag.Inmueble = new SelectList(
                repoInmueble.GetInmuebles(), //metodo para obtener la lista de inmuebles
                "ID_inmueble", 
                "Direccion",
                contrato.ID_inmueble
            );

            ViewBag.Inquilino = new SelectList(
                repoInquilino.GetInquilinos(), //metodo para obtener la lista de inquilinos
                "ID_inquilino", 
                "Nombre",
                contrato.ID_inquilino
            );

            return View(contrato);
        }

        //eliminar contrato (logico)
        public IActionResult Eliminar(int id)
        {
            repoContrato.Eliminar(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
