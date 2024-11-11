using inmobiliariaAST.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliariaAST.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly ILogger<PagoController> _logger;
        private readonly RepositorioPago repo;
        private readonly RepositorioContrato repoContrato;

        public PagoController(ILogger<PagoController> logger)
        {
            _logger = logger;
            repo = new RepositorioPago();
            repoContrato = new RepositorioContrato();
        }

        public IActionResult Index(string estado){
            
            bool? estadoSelect = null;
            if(bool.TryParse(estado, out bool res))
            {
                estadoSelect = res;
            }

            ViewBag.EstadoSelect = estadoSelect;
            var pagos = repo.GetPagos(estadoSelect);
            return View(pagos);
        }

        //creacion
        [HttpGet]
        public IActionResult Crear(){
            //ViewBag.Contratos = new SelectList(repoContrato.GetContratos(), "ID_contrato");
            var contratos = repoContrato.Get().Select(c => new
            {
                c.ID_contrato,
                // DescripcionContrato = $"{c.InmuebleDireccion} - {c.InquilinoNombreCompleto} (Inicio: {c.Fecha_Inicio.ToShortDateString()})"
            }).ToList();
            
           ViewBag.Contratos = new SelectList(contratos, "ID_contrato", "DescripcionContrato");

            return View();
        }

        [HttpPost]
        public IActionResult Crear(Pago pago){{
            if (ModelState.IsValid)
            {
                int numeroPago = repo.GetProximoNumPago(pago.ID_contrato);
                pago.Numero_pago = numeroPago;


                pago.Estado = true;
                repo.Alta(pago);
                 TempData["SuccessMessage"] = "Pago agregado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            
            var contratos = repoContrato.Get().Select(c => new
            {
                c.ID_contrato,
                // DescripcionContrato = $"{c.InmuebleDireccion} - {c.InquilinoNombreCompleto} (Inicio: {c.Fecha_Inicio.ToShortDateString()})"
            }).ToList();
            
           ViewBag.Contratos = new SelectList(contratos, "ID_contrato", "DescripcionContrato");

            return View(pago);
            }
        }

        //edicion
        public IActionResult Edicion(int id)
        {
            var pago = repo.Get(id);
            if (pago == null){
                return NotFound();
            }
            return View(pago);
        }

        [HttpPost]
        public IActionResult Edicion(Pago pago)
        {
            if (ModelState.IsValid)
            {
                repo.EditarPago(pago);
                TempData["SuccessMessage"] = "Cambios guardados exitosamente";
                return RedirectToAction("Index", new {id = pago.ID_contrato});
            }
            return View();
        }

        //detalle
        public IActionResult Detalles(int id)
        {
            var pago = repo.Get(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }



        //eliminacion logica
        [Authorize(Roles = "Administrador")]
        public IActionResult Eliminar(int id){
            
            var resultado = repo.AnularPago(id);
            if(resultado >0){
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View("error anulacion de pago");
            }
        }
    }
}