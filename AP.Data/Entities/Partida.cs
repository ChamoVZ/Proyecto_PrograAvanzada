using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.Data.Entities
{
    // Registro de cada intento de un jugador sobre un reto (historial y marcadores)
    public class Partida
    {
        [Key]
        public int PartidaId { get; set; }

        // Identity maneja UserId como string GUID de 128 chars
        [Required, StringLength(128)]
        public string UsuarioId { get; set; }

        public int RetoId { get; set; }

        [ForeignKey(nameof(RetoId))]
        public virtual Reto Reto { get; set; }

        public bool Acertado { get; set; }

        public int TiempoEmpleadoSegundos { get; set; }

        // XP se calcula en AP.Core según dificultad y tiempo; aquí solo se almacena
        public int XpGanado { get; set; }

        public DateTime FechaJuego { get; set; }
    }
}
