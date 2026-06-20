using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.Data.Entities
{
    /// <summary>
    /// Entidad de dominio: el intento de un usuario sobre un reto.
    /// Registra resultado, tiempo y XP ganado (alimenta marcadores e insignias).
    /// </summary>
    public class Partida
    {
        [Key]
        public int PartidaId { get; set; }

        /// <summary>Id del usuario de Identity (string, como lo maneja ASP.NET Identity).</summary>
        [Required, StringLength(128)]
        public string UsuarioId { get; set; }

        public int RetoId { get; set; }

        [ForeignKey(nameof(RetoId))]
        public virtual Reto Reto { get; set; }

        public bool Acertado { get; set; }

        /// <summary>Segundos que tardó en responder.</summary>
        public int TiempoEmpleadoSegundos { get; set; }

        /// <summary>XP otorgado por esta partida (calculado en AP.Core, no aquí).</summary>
        public int XpGanado { get; set; }

        public DateTime FechaJuego { get; set; }
    }
}
