using System.ComponentModel.DataAnnotations;

namespace InventorySys.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }

        [Required(ErrorMessage = "Debes aceptar la Política de Privacidad para continuar")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Debes aceptar la Política de Privacidad")]
        public bool AcceptPrivacyPolicy { get; set; }
    }
}
