using System;
using System.ComponentModel.DataAnnotations;

namespace AP.Data.Entities
{
    public enum EstadoSolicitud
    {
        Abierta = 1,
        EnProceso = 2,
        Cerrada = 3
    }

    // Entidad EF para el modulo de Solicitud TI
    public class SolicitudTI
    {
        [Key]
        public int SolicitudTIId { get; set; }

        [Required]
        [StringLength(128)]
        public string UsuarioId { get; set; } // Identificador del usuario (Identity)

        [Required]
        [StringLength(200)]
        public string Asunto { get; set; }

        [Required]
        [StringLength(2000)]
        public string Descripcion { get; set; }

        public EstadoSolicitud Estado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public bool Activo { get; set; }

        // Auditoria (mismo patron que Publicacion)
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string ModifiedBy { get; set; }
    }
}
