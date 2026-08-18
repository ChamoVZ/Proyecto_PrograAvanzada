using System.ComponentModel.DataAnnotations;

namespace AP.Models.Juegos
{
    public class ResponderSecuenciasLogicasViewModel
    {
        [Required]
        public int RetoId { get; set; }

        [Required(ErrorMessage = "Ingrese el siguiente número de la secuencia.")]
        [StringLength(100)]
        [Display(Name = "Respuesta")]
        public string RespuestaUsuario { get; set; }
    }
}
