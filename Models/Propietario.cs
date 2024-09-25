namespace inmobiliariaAST.Models;

public class Propietario
{
    public int ID_propietario { get; set; } 

    public string DNI { get; set; } = "";
    
    public string Nombre { get; set; } = "";

    public string Apellido { get; set; } = "";

    public string Telefono { get; set; } = "";

    public string Email { get; set; } = "";

    public string Direccion { get; set; } = "";

    public bool Estado { get; set; }
}
