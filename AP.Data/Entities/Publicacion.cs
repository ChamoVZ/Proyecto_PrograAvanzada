using System;
using System.ComponentModel.DataAnnotations;

namespace AP.Data.Entities
{
    // Entidad EF para el módulo de Foro
    public class Publicacion
    {
        [Key]
        public int PublicacionId { get; set; }

        [Required]
        [StringLength(128)]
        public string UsuarioId { get; set; } // Identificador del usuario (Identity)

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(2000)]
        public string Contenido { get; set; }

        public DateTime FechaPublicacion { get; set; }

        public bool Activo { get; set; }

        // Auditoría (mismo patrón que en Reto)
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string ModifiedBy { get; set; }
    }
}
