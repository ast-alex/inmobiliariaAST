namespace inmobiliariaAST.Models;

using System.ComponentModel.DataAnnotations;

public class Inquilino
{
    [Key]
    public int ID_inquilino { get; set; }
    public string DNI { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string Apellido { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string Email { get; set; } = "";
    public string Direccion { get; set; } = "";
    public bool Estado { get; set; }
}
