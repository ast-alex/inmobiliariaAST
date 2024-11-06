namespace inmobiliariaAST.Models;

using System.ComponentModel.DataAnnotations;

public class Pago
{   
    [Key]
    public int ID_pago { get; set; }
    public int ID_contrato {get; set;}
    public int Numero_pago {get; set;}
    public DateTime Fecha_pago {get; set;}
    public decimal Importe {get; set;}
    public String? Concepto {get; set;}
    public bool Estado {get; set;}

     public string? InquilinoNombreCompleto { get; set; }
    public string? InmuebleDireccion { get; set; }
}