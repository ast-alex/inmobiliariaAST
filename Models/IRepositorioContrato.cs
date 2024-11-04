namespace inmobiliariaAST.Models
{
    public interface IRepositorioContrato
    {
        IEnumerable<Contrato> ListarContratosPorPropietario(int idPropietario);
        Contrato GetId(int id);
        Contrato GetDetalle(int idInmueble, int idPropietario);
    }
}