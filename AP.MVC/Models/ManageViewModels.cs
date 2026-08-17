using System.ComponentModel.DataAnnotations;

namespace AP.MVC.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "La contraseña nueva es obligatoria.")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña nueva")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña nueva")]
        [Compare("NewPassword", ErrorMessage = "La contraseña nueva y su confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}
