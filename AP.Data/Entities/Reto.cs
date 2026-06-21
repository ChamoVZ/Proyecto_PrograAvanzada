using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AP.Data.Entities
{
    // Modos disponibles: 1=OperadorPerdido, 2=Contrarreloj, 3=SecuenciasLogicas
    public enum ModoJuego
    {
        OperadorPerdido = 1,
        Contrarreloj = 2,
        SecuenciasLogicas = 3
    }

    // Entidad EF — solo datos, nunca va directo a las vistas (usar ViewModels de AP.Models)
    public class Reto
    {
        [Key]
        public int RetoId { get; set; }

        [Required, StringLength(120)]
        public string Titulo { get; set; }

        public ModoJuego Modo { get; set; }

        // Ejemplo: "7 _ 3 = 21"
        [Required, StringLength(500)]
        public string Enunciado { get; set; }

        [Required, StringLength(100)]
        public string RespuestaCorrecta { get; set; }

        // Rango 1-5; el XP base se calcula en AP.Core según este valor
        [Range(1, 5)]
        public int Dificultad { get; set; }

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
