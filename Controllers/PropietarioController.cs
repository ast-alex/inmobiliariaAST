using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaAST.Models;

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
        id = propietario.Id_propietario;
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
    
    public IActionResult Eliminar(int id)
    {
        repo.Baja(id);
        return RedirectToAction(nameof(Index));
    }
}
