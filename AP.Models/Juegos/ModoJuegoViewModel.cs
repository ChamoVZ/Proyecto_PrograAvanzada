namespace AP.Models.Juegos
{
    /// <summary>Datos de un modo de juego mostrados en la pantalla de seleccion.</summary>
    public class ModoJuegoViewModel
    {
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        // Clase de Bootstrap Icons, por ejemplo "bi-stopwatch".
        public string Icono { get; set; }

        // Controlador que atiende el modo; queda nulo si todavia no existe.
        public string Controlador { get; set; }

        public bool Disponible { get; set; }
    }
}
