using System;
using System.ComponentModel.DataAnnotations;
namespace AP.Models.Comunidad
{
    public class PublicacionViewModel
    {
        // La resuelve ForoBusiness y la consulta la vista, para no repetir la regla en el Razor.
        public bool PuedeModificar { get; set; }

        public int PublicacionId { get; set; }

        public string UsuarioId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(200, ErrorMessage = "El título no puede exceder los 200 caracteres.")]
        [Display(Name = "Título de la publicación")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El contenido es obligatorio.")]
        [StringLength(2000, ErrorMessage = "El contenido no puede exceder los 2000 caracteres.")]
        [Display(Name = "Contenido")]
        // [AllowHtml] // Descomentar si luego se integra un editor WYSIWYG
        public string Contenido { get; set; }

        [Display(Name = "Fecha de Publicación")]
        public DateTime FechaPublicacion { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; }
        
        // Propiedad adicional útil para la vista
        [Display(Name = "Autor")]
        public string NombreAutor { get; set; }
    }
}


