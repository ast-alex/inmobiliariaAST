using System.Collections.Generic;

namespace inmobiliariaAST.Models
{
    public interface IRepositorio<T>
    {
        List<T> Get(); // Método para obtener todos
        T GetId(int id); // Método para obtener por ID
        int Alta(T entity); // Método para agregar una nueva entidad
        int Modificar(T entity); // Método para modificar una entidad existente
        int Baja(int id); // Método para dar de baja una entidad

    }
}
