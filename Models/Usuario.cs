namespace inmobiliariaAST.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Usuario{
        
        public int ID_usuario { get; set; }
        public string? Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }
        public int Rol { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public bool Estado { get; set; } = true;
        public string? Avatar { get; set; }
        [NotMapped]
        public IFormFile? AvatarFile { get; set; }

        public const string AvatarDefault = "/uploads/avatars/default.jpg";

}

public enum Roles{
    Administrador = 1,
    Empleado = 2
}