using System;
using System.ComponentModel.DataAnnotations;

namespace AP.Models.Buzon
{
    public class QuejaViewModel
    {
        public int QuejaId { get; set; }

        public string UsuarioId { get; set; }

        [Required(ErrorMessage = "El asunto es obligatorio.")]
        [StringLength(200, ErrorMessage = "El asunto no puede exceder los 200 caracteres.")]
        [Display(Name = "Asunto")]
        public string Asunto { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(2000, ErrorMessage = "La descripción no puede exceder los 2000 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Range(1, 5, ErrorMessage = "Selecciona una categoría válida.")]
        [Display(Name = "Categoría")]
        public int Categoria { get; set; }

        [Display(Name = "Estado")]
        public int Estado { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Autor")]
        public string NombreAutor { get; set; }

        [Display(Name = "Categoría")]
        public string NombreCategoria { get; set; }

        [Display(Name = "Estado")]
        public string NombreEstado { get; set; }
    }
}
