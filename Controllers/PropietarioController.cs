using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using inmobiliariaAST.Services;


namespace inmobiliariaAST.Models;

[Authorize]
public class PropietarioController : Controller
{
    private readonly ILogger<PropietarioController> _logger;
    private readonly AuthenticationService _authService;

    private RepositorioPropietario repo;
    public PropietarioController(ILogger<PropietarioController> logger, AuthenticationService authService)
    {
        _logger = logger;
        _authService = authService;

        repo = new RepositorioPropietario();
    }

    public IActionResult Index()
    {
        var lista = repo.Get();
        return View(lista);
    }
     
    public IActionResult Edicion(int id)
    {
        if(id == 0)
        return View();
        else
        {
            var propietario = repo.GetId(id);
            TempData["SuccessMessage"] = "Cambios guardados exitosamente";
            return View(propietario);
        }
    }

    [HttpPost]
    public IActionResult Guardar(int id, Propietario propietario)
    {
        id = propietario.ID_propietario;
        if(id == 0)
        {
            if (!string.IsNullOrEmpty(propietario.Password))
            {
                propietario.Password = _authService.HashPassword(propietario.Password);
            }
            repo.Alta(propietario);
        }
        else
        {
            repo.Modificar(propietario);
        }

        return RedirectToAction(nameof(Index));
    }
    //detalle propietario
    public IActionResult Detalles(int id){
        
        var propietario = repo.GetId(id);
        if(propietario == null){
            return NotFound();
        }
        return View(propietario);
    }

    [Authorize(Roles = "Administrador")]    
    public IActionResult Eliminar(int id)
    {
        var propietario = repo.GetId(id);
        if(propietario == null){
            return NotFound();
        }

        repo.Baja(id);
        TempData["SuccessMessage"] = "El propietario ha sido dado de baja con sus activos relacionados.";
        return RedirectToAction(nameof(Index));
    }
}
