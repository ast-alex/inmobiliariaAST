using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaAST.Models{
    public class PerfilViewModel{
        public string? DNI { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        
        public string? Avatar { get; set; }
        public IFormFile? AvatarFile { get; set; }
    }
}