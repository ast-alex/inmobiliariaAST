using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inmobiliariaAST.Models{

    public interface IRepositorioPropietario : IRepositorio<Propietario>
    {
        Propietario? GetByEmail(string email);
    }
}