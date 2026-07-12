using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP.Models.Juegos
{
    public class ResponderRetoViewModel
    {
        [Required] 
        public int RetoId { get; set; }

        [Required (ErrorMessage = "Seleccione un operador.")]
        [Display(Name = "Operador")]
        public string RespuestaUsuario { get; set; }

        [Required]
        public long HoraInicioTicks { get; set; }
    }
}
