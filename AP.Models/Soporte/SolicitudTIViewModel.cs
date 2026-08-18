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

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(2000, ErrorMessage = "La descripción no puede exceder los 2000 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Estado")]
        public int Estado { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Solicitante")]
        public string NombreSolicitante { get; set; }

        [Display(Name = "Estado")]
        public string NombreEstado { get; set; }

        // Los resuelve el controller preguntandole a SolicitudTIBusiness, para que la vista
        // no vuelva a escribir la regla de autorizacion.
        public bool PuedeEditar { get; set; }

        public bool PuedeEliminar { get; set; }
    }
}
