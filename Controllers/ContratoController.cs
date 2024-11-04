using inmobiliariaAST.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliariaAST.Controllers{

    [Authorize]
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
            ViewBag.Inmuebles = new SelectList(
                repoInmueble.Get(), //metodo para obtener la lista de inmuebles
                "ID_inmueble", 
                "Direccion"
            );

            // Obtener la lista de inquilinos y concatenar nombre y apellido
            var inquilinos = repoInquilino.GetInquilinos()
                .Select(i => new SelectListItem
                {
                    Value = i.ID_inquilino.ToString(),
                    Text = $"{i.Nombre} {i.Apellido}", // Concatenar nombre y apellido
                })
                .ToList();

            ViewBag.Inquilinos = inquilinos;

            return View();
        }

        //envio del form de crear contrato
        [HttpPost]
        public IActionResult Crear(Contrato contrato)
        {
            if(ModelState.IsValid)
            {
                //al crear un contrato deberia inicializarse como ACTIVO por defecto...
                contrato.Estado = true;
                repoContrato.Alta(contrato);
                TempData["SuccessMessage"] = "Contrato creado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            //si no es valido, volvemos a mostrar el form
            ViewBag.Inmuebles = new SelectList(
                repoInmueble.Get(), 
                "ID_inmueble", 
                "Direccion"
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
            ViewBag.Inmuebles = new SelectList(
                repoInmueble.Get(),
                "ID_inmueble", 
                "Direccion",
                contrato.ID_inmueble
            );

            // Obtener la lista de inquilinos y concatenar nombre y apellido
            var inquilinos = repoInquilino.GetInquilinos()
                .Select(i => new SelectListItem
                {
                    Value = i.ID_inquilino.ToString(),
                    Text = $"{i.Nombre} {i.Apellido}", // Concatenar nombre y apellido
                })
                .ToList();

            ViewBag.Inquilinos = new SelectList(
                inquilinos,
                "Value",
                "Text", 
                contrato.ID_inquilino.ToString() // Seleccionar el inquilino actual
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
                TempData["SuccessMessage"] = "Cambios guardados exitosamente";
                return RedirectToAction(nameof(Index));
            }

            //si no es valido, volvemos a mostrar el form
            ViewBag.Inmuebles = new SelectList(
                repoInmueble.Get(), 
                "ID_inmueble", 
                "Direccion",
                contrato.ID_inmueble
            );

            ViewBag.Inquilinos = new SelectList(
                repoInquilino.GetInquilinos(), 
                "ID_inquilino", 
                "InquilinoNombreCompleto",
                contrato.ID_inquilino
            );

            return View(contrato);
        }

        //detalles contrato
        public IActionResult Detalles(int id)
        {
            var contrato = repoContrato.Get(id);
            if(contrato == null)
            {
                return NotFound();
            }
            return View(contrato);
        }



        //eliminar contrato (logico)
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            repoContrato.Eliminar(id);
            TempData["SuccessMessage"] = "Contrato dado de baja correctamente";
            return RedirectToAction(nameof(Index));
        }

    }
}
