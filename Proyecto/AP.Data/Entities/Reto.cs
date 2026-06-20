using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AP.Data.Entities
{
    /// <summary>
    /// Modos de juego de MathemaX.
    /// </summary>
    public enum ModoJuego
    {
        OperadorPerdido = 1,
        Contrarreloj = 2,
        SecuenciasLogicas = 3
    }

    /// <summary>
    /// Entidad de dominio: un reto matemático que el jugador resuelve.
    /// Entidad de EF = espejo de la tabla. NUNCA viaja a las vistas
    /// (para eso están los ViewModels de AP.Models).
    /// </summary>
    public class Reto
    {
        [Key]
        public int RetoId { get; set; }

        [Required, StringLength(120)]
        public string Titulo { get; set; }

        public ModoJuego Modo { get; set; }

        /// <summary>Enunciado o definición del reto (ej. "7 _ 3 = 21").</summary>
        [Required, StringLength(500)]
        public string Enunciado { get; set; }

        [Required, StringLength(100)]
        public string RespuestaCorrecta { get; set; }

        /// <summary>Dificultad 1..5; determina el XP base otorgado.</summary>
        [Range(1, 5)]
        public int Dificultad { get; set; }

        /// <summary>Segundos disponibles para resolverlo (cronómetro).</summary>
        public int TiempoLimiteSegundos { get; set; }

        public bool Activo { get; set; }

        // Auditoría (mismo patrón del proyecto de referencia del profesor)
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string ModifiedBy { get; set; }

        // Navegación: un reto tiene muchas partidas jugadas.
        public virtual ICollection<Partida> Partidas { get; set; }
    }
}
