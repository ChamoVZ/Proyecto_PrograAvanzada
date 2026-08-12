using System;

namespace AP.Models.Juegos
{
    public class HistorialPartidaViewModel
    {
        public DateTime FechaJuego { get; set; }
        public bool Acertado { get; set; }
        public int TiempoEmpleadoSegundos { get; set; }
        public int XpGanado { get; set; }
    }
}
