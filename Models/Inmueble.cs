namespace inmobiliariaAST.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Inmueble{
    [Key]
    public int ID_inmueble { get; set; }
    [Required]
    public string? Direccion { get; set; }
    [Required]
    public UsoInmueble Uso { get; set; }
    [Required]
    public TipoInmueble Tipo { get; set; }
    [Required]
    public int Cantidad_Ambientes { get; set; }
    [Required]
    public decimal? Latitud { get; set; }
    [Required]
    public decimal? Longitud { get; set; }
    [Required]
    public decimal Precio { get; set; }
    [Required]
    public bool Estado { get; set; } = true;
    public bool Disponibilidad { get; set; } 
    [Required]
    public int ID_propietario { get; set; }
    public Propietario? Propietario { get; set; }
    public string? Foto { get; set; }
   [NotMapped]
    public IFormFile? FotoFile { get; set; }

    
}

public enum UsoInmueble{
    Comercial,
    Residencial
}

public enum TipoInmueble{
    Local,
    Deposito,
    Casa,
    Departamento,
    Otro
}
