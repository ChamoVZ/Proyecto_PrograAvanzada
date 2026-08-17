using System;
using System.ComponentModel.DataAnnotations;
using AP.Data.Entities;

namespace AP.Models.Juegos
{
    public class FiltroHistorialViewModel
    {
        [Display(Name = "Resultado")]
        public bool? Resultado { get; set; }

        [Display(Name = "Modo de juego")]
        public ModoJuego? Modo { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Desde")]
        public DateTime? Desde { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Hasta")]
        public DateTime? Hasta { get; set; }
    }
}
