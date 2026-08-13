using System;
using System.ComponentModel.DataAnnotations;

namespace AP.Data.Entities
{
    // Estados por los que transita una queja dentro del buzon.
    public enum EstadoQueja
    {
        Pendiente = 1,
        EnRevision = 2,
        Resuelta = 3
    }

    // Categoria que clasifica el motivo de la queja (facilita el triage de soporte).
    public enum CategoriaQueja
    {
        Bug = 1,
        Contenido = 2,
        Cuenta = 3,
        Sugerencia = 4,
        Otro = 5
    }

    // Entidad EF para el modulo de Buzon de Quejas.
    // Mismo patron de auditoria que Publicacion y SolicitudTI.
    public class Queja
    {
        [Key]
        public int QuejaId { get; set; }

        [Required]
        [StringLength(128)]
        public string UsuarioId { get; set; } // Identificador del usuario (Identity)

        [Required]
        [StringLength(200)]
        public string Asunto { get; set; }

        [Required]
        [StringLength(2000)]
        public string Descripcion { get; set; }

        public CategoriaQueja Categoria { get; set; }

        public EstadoQueja Estado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public bool Activo { get; set; }

        // Auditoria (mismo patron que Publicacion y SolicitudTI)
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string ModifiedBy { get; set; }
    }
}
