using System.ComponentModel.DataAnnotations;

public class ResetPasswordViewModel{
    
    [Required]
    [DataType(DataType.Password)]
    public string? NuevaPassword {get;set;}

    [Required]
    [DataType(DataType.Password)]
    [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden")]
    public string? ConfirmarPassword {get;set;}

}


