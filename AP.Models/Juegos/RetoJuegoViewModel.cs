using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AP.Models.Juegos
{
    public class RetoJuegoViewModel
    {
        public int RetoId { get; set; }
        public string Enunciado { get; set; }
        public int Dificultad { get; set; }
        public int TiempoLimiteSegundos { get; set; }
        public long HoraInicioTicks { get; set; }
    }
}
