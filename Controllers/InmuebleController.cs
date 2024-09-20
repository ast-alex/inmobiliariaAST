using inmobiliariaAST.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliariaAST.Controllers
{
    [Authorize]
    public class InmuebleController : Controller
    {
        private readonly ILogger<InmuebleController> _logger;
        private readonly RepositorioInmueble repo;
        private readonly RepositorioPropietario repoPropietario; 

        public InmuebleController(ILogger<InmuebleController> logger)
        {
            _logger = logger;
            repo = new RepositorioInmueble();
            repoPropietario = new RepositorioPropietario(); 
        }

        // Listar todos los inmuebles
        public IActionResult Index()
        {
            var lista = repo.GetInmuebles();
            return View(lista);
        }

        // Mostrar el formulario de creación
        public IActionResult Crear()
        {
            var propietarios = repoPropietario.GetPropietarios()
                .Select(p => new SelectListItem{
                    Value = p.ID_propietario.ToString(),
                    Text = $"{p.Nombre} {p.Apellido}"
                }).ToList();

            // Cargar lista de propietarios para el dropdown en la vista
            ViewBag.Propietarios = propietarios;

            return View();
        }

        // Manejar el envío del formulario de creación
        [HttpPost]
        public IActionResult Crear(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                repo.Alta(inmueble); // Usamos repo.Alta para insertar un nuevo inmueble
                return RedirectToAction(nameof(Index));
            }

            // Volver a mostrar el formulario con errores si el modelo no es válido
            ViewBag.Propietario = new SelectList(
                repoPropietario.GetPropietarios(),
                "ID_propietario",
                "Nombre"
            );

            return View(inmueble);
        }

        [HttpGet]
        public IActionResult Edicion(int id)
        {
            // Obtener el inmueble con su propietario
            var inmueble = repo.Get(id);
            
            if (inmueble == null)
            {
                return NotFound();
            }

            var propietarios = repoPropietario.GetPropietarios()
                .Select(p => new SelectListItem{
                    Value = p.ID_propietario.ToString(),
                    Text = $"{p.Nombre} {p.Apellido}"
                }).ToList();

            // Cargar lista de propietarios para el dropdown en la vista
            ViewBag.Propietarios = propietarios;

            return View(inmueble);
        }



        //envio del formulario de edicion
        [HttpPost]
        public IActionResult Edicion(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                // Llamar al repositorio para modificar el inmueble
                repo.Modificar(inmueble);
                return RedirectToAction("Index"); 
            }

            var propietarios = repoPropietario.GetPropietarios()
                .Select(p => new SelectListItem
                {
                    Value = p.ID_propietario.ToString(),
                    Text = $"{p.Nombre} {p.Apellido}"
                }).ToList();

            // En caso de error, recargar la lista de propietarios para el dropdown
            ViewBag.Propietarios = repoPropietario.GetPropietarios();

            return View(inmueble);
        }

        //detalles inmueble
        public IActionResult Detalles(int id){

            var inmueble = repo.Get(id);
            if(inmueble == null){
                return NotFound();
            }
            return View(inmueble);
        }

        [Authorize(Roles = "Administrador")]
        // Eliminar inmueble
        public IActionResult Eliminar(int id)
        {
            repo.Baja(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
