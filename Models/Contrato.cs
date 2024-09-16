namespace inmobiliariaAST.Models;

public class Contrato{
    public int ID_contrato { get; set; }
    public int ID_inmueble { get; set; }
    public int ID_inquilino { get; set; }
    public DateTime Fecha_Inicio { get; set; }
    public DateTime Fecha_Fin { get; set; }
    public decimal Monto_Mensual { get; set; }
    public bool Estado { get; set; }
    public DateTime? Fecha_Terminacion_Anticipada { get; set; }
    public decimal? Multa { get; set; }

}