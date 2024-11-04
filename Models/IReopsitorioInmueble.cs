using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inmobiliariaAST.Models{
    public interface IRepositorioInmueble : IRepositorio<Inmueble>
    {
        IList<Inmueble> GetInmueblesPorPropietario(int idPropietario);
        Inmueble GetDetalleInmueble(int id);
        
    }
}