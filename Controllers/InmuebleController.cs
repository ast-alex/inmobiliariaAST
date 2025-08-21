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
            var lista = repo.Get();
            return View(lista);
        }

        // Mostrar el formulario de creación
        public IActionResult Crear()
        {
            var propietarios = repoPropietario.Get()
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
                TempData["SuccessMessage"] = "Inmueble creado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            // Volver a mostrar el formulario con errores si el modelo no es válido
            ViewBag.Propietario = new SelectList(
                repoPropietario.Get(),
                "ID_propietario",
                "Nombre"
            );

            return View(inmueble);
        }

        [HttpGet]
        public IActionResult Edicion(int id)
        {
            // Obtener el inmueble con su propietario
            var inmueble = repo.GetId(id);
            
            if (inmueble == null)
            {
                return NotFound();
            }

            var propietarios = repoPropietario.Get()
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
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Hay campos vacíos. Complete el formulario.";

                // Recargar lista de propietarios para el dropdown
                ViewBag.Propietarios = repoPropietario.Get()
                    .Select(p => new SelectListItem
                    {
                        Value = p.ID_propietario.ToString(),
                        Text = $"{p.Nombre} {p.Apellido}"
                    }).ToList();

                return View(inmueble);
            }

            // Buscar el inmueble existente
            var inmuebleExists = repo.GetId(inmueble.ID_inmueble);
            if (inmuebleExists == null)
            {
                TempData["ErrorMessage"] = "No se encontró el inmueble.";

                ViewBag.Propietarios = repoPropietario.Get()
                    .Select(p => new SelectListItem
                    {
                        Value = p.ID_propietario.ToString(),
                        Text = $"{p.Nombre} {p.Apellido}"
                    }).ToList();

                return View(inmueble);
            }

            // Verificar si hay cambios reales
            bool hayCambios =
                inmuebleExists.Direccion != inmueble.Direccion ||
                inmuebleExists.Uso != inmueble.Uso ||
                inmuebleExists.Tipo != inmueble.Tipo ||
                inmuebleExists.Cantidad_Ambientes != inmueble.Cantidad_Ambientes ||
                inmuebleExists.Latitud != inmueble.Latitud ||
                inmuebleExists.Longitud != inmueble.Longitud ||
                inmuebleExists.Precio != inmueble.Precio ||
                inmuebleExists.Disponibilidad != inmueble.Disponibilidad ||
                inmuebleExists.ID_propietario != inmueble.ID_propietario ||
                (User.IsInRole("Administrador") && inmuebleExists.Estado != inmueble.Estado);

            if (!hayCambios)
            {
                TempData["ErrorMessage"] = "No se detectaron cambios para guardar.";
                ViewBag.Propietarios = repoPropietario.Get()
                    .Select(p => new SelectListItem
                    {
                        Value = p.ID_propietario.ToString(),
                        Text = $"{p.Nombre} {p.Apellido}"
                    }).ToList();

                return View(inmueble);
            }

            // Actualizar campos
            inmuebleExists.Direccion = inmueble.Direccion;
            inmuebleExists.Uso = inmueble.Uso;
            inmuebleExists.Tipo = inmueble.Tipo;
            inmuebleExists.Cantidad_Ambientes = inmueble.Cantidad_Ambientes;
            inmuebleExists.Latitud = inmueble.Latitud;
            inmuebleExists.Longitud = inmueble.Longitud;
            inmuebleExists.Precio = inmueble.Precio;
            inmuebleExists.Disponibilidad = inmueble.Disponibilidad;
            inmuebleExists.ID_propietario = inmueble.ID_propietario;

            if (User.IsInRole("Administrador"))
                inmuebleExists.Estado = inmueble.Estado;

            // Guardar cambios
            repo.Modificar(inmuebleExists);

            TempData["SuccessMessage"] = "Cambios guardados exitosamente";
            return RedirectToAction("Index");
        }


        //detalles inmueble
        public IActionResult Detalles(int id){

            var inmueble = repo.GetId(id);
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
