using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace inmobiliariaAST.Models;

[Authorize]
public class PropietarioController : Controller
{
    private readonly ILogger<PropietarioController> _logger;

    private RepositorioPropietario repo;
    public PropietarioController(ILogger<PropietarioController> logger)
    {
        _logger = logger;

        repo = new RepositorioPropietario();
    }

    public IActionResult Index()
    {
        var lista = repo.GetPropietarios();
        return View(lista);
    }
     
    public IActionResult Edicion(int id)
    {
        if(id == 0)
        return View();
        else
        {
            var propietario = repo.Get(id);
            return View(propietario);
        }
    }

    [HttpPost]
    public IActionResult Guardar(int id, Propietario propietario)
    {
        id = propietario.ID_propietario;
        if(id == 0)
        {
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
        
        var propietario = repo.Get(id);
        if(propietario == null){
            return NotFound();
        }
        return View(propietario);
    }

    [Authorize(Roles = "Administrador")]    
    public IActionResult Eliminar(int id)
    {
        repo.Baja(id);
        return RedirectToAction(nameof(Index));
    }
}
