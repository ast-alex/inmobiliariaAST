namespace inmobiliariaAST.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Propietario
{   
    [Key]
    public int ID_propietario { get; set; } 

    public string DNI { get; set; } = "";
    
    public string Nombre { get; set; } = "";

    public string Apellido { get; set; } = "";

    public string Telefono { get; set; } = "";

    public string Email { get; set; } = "";
    
    [Required, DataType(DataType.Password)]
    public string? Password { get; set; }

    public string Direccion { get; set; } = "";

    public bool Estado { get; set; }

    public string? Avatar { get; set; }
    [NotMapped]
    public IFormFile? AvatarFile { get; set; }
    public const string AvatarDefault = "/uploads/avatars/default.jpg";

    
}
