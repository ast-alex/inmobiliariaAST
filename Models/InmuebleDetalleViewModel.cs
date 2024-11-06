namespace Inmobiliaria.Models;


public class InmuebleDetalleViewModel{
    public int ID_inmueble { get; set; }
    
    public string? Direccion { get; set; }
    
    public string? Uso { get; set; }
    
    public string? Tipo { get; set; }
    
    public int Cantidad_Ambientes { get; set; }
    
    public decimal? Latitud { get; set; }
    
    public decimal? Longitud { get; set; }
    
    public decimal Precio { get; set; }
    
    public bool Disponibilidad { get; set; } 
    
    public string? Foto { get; set; }

}