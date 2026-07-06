using System;
using System.ComponentModel.DataAnnotations;

namespace AP.Models.Soporte
{
    public class SolicitudTIViewModel
    {
        public int SolicitudTIId { get; set; }

        public string UsuarioId { get; set; }

        [Required(ErrorMessage = "El asunto es obligatorio.")]
        [StringLength(200, ErrorMessage = "El asunto no puede exceder los 200 caracteres.")]
        [Display(Name = "Asunto")]
        public string Asunto { get; set; }

        [Required(ErrorMessage = "La descripci\u00F3n es obligatoria.")]
        [StringLength(2000, ErrorMessage = "La descripci\u00F3n no puede exceder los 2000 caracteres.")]
        [Display(Name = "Descripci\u00F3n")]
        public string Descripcion { get; set; }

        [Display(Name = "Estado")]
        public int Estado { get; set; }

        [Display(Name = "Fecha de Creaci\u00F3n")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Solicitante")]
        public string NombreSolicitante { get; set; }

        [Display(Name = "Estado")]
        public string NombreEstado { get; set; }
    }
}
