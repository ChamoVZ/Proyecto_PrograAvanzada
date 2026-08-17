using AP.Data.Entities;

namespace AP.Core.Business.Estrategias
{
    // DP: Strategy - contrato comun de cada modo de juego; el contexto lo usa sin conocer la clase concreta.
    public interface IModoJuegoStrategy
    {
        ModoJuego Modo { get; }
        bool ValidarRespuesta(Reto reto, string respuestaUsuario);
        int CalcularXp(Reto reto, bool acertado, int tiempoEmpleadoSegundos);
    }
}
