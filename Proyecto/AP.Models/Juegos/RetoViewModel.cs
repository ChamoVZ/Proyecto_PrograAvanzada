using System.ComponentModel.DataAnnotations;

namespace AP.Models.Juegos
{
    /// <summary>
    /// ViewModel de Reto: lo ÚNICO que llega a las vistas Razor (Regla de Oro:
    /// las entidades de EF nunca salen de las capas inferiores).
    /// Las DataAnnotations alimentan la validación de MVC (cliente y servidor).
    /// </summary>
    public class RetoViewModel
    {
        public int RetoId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(120)]
        [Display(Name = "Título del reto")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Seleccione un modo de juego.")]
        [Display(Name = "Modo de juego")]
        public int Modo { get; set; }

        [Required(ErrorMessage = "El enunciado es obligatorio.")]
        [StringLength(500)]
        [Display(Name = "Enunciado")]
        public string Enunciado { get; set; }

        [Required(ErrorMessage = "Indique la respuesta correcta.")]
        [StringLength(100)]
        [Display(Name = "Respuesta correcta")]
        public string RespuestaCorrecta { get; set; }

        [Range(1, 5, ErrorMessage = "La dificultad va de 1 a 5.")]
        [Display(Name = "Dificultad")]
        public int Dificultad { get; set; }

        [Range(5, 600, ErrorMessage = "El tiempo límite debe estar entre 5 y 600 segundos.")]
        [Display(Name = "Tiempo límite (segundos)")]
        public int TiempoLimiteSegundos { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }
    }
}
