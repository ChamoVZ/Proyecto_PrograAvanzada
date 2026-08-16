using System.ComponentModel.DataAnnotations;

namespace AP.Models.Juegos
{
    public class ResponderContrarrelojViewModel
    {
        [Required]
        public int RetoId { get; set; }

        [Required(ErrorMessage = "Ingrese una respuesta.")]
        [StringLength(100)]
        [Display(Name = "Respuesta")]
        public string RespuestaUsuario { get; set; }
    }
}
