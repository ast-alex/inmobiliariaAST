namespace inmobiliariaAST.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Contrato{

    [Key]
    public int ID_contrato { get; set; }
    [ForeignKey("Inmueble")]
    public int ID_inmueble { get; set; }
    [ForeignKey("Inquilino")]
    public int ID_inquilino { get; set; }
    public DateTime Fecha_Inicio { get; set; }
    public DateTime Fecha_Fin { get; set; }
    public decimal Monto_Mensual { get; set; }
    public bool Estado { get; set; }
    public DateTime? Fecha_Terminacion_Anticipada { get; set; }
    public decimal? Multa { get; set; }


    public Inmueble? Inmueble { get; set; }
    public Inquilino? Inquilino { get; set; }

}