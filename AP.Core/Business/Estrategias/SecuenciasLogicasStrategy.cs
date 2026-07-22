using AP.Data.Entities;

namespace AP.Core.Business.Estrategias
{
    public class SecuenciasLogicasStrategy : IModoJuegoStrategy
    {
        // Este modo premia un poco mas porque exige deducir el patron.
        private const int XpPorDificultad = 12;

        public ModoJuego Modo => ModoJuego.SecuenciasLogicas;

        public bool ValidarRespuesta(Reto reto, string respuestaUsuario)
        {
            // La respuesta es el siguiente numero de la secuencia, se compara como entero.
            if (!int.TryParse((respuestaUsuario ?? string.Empty).Trim(), out var respuesta))
                return false;

            if (!int.TryParse(reto.RespuestaCorrecta.Trim(), out var correcta))
                return false;

            return respuesta == correcta;
        }

        public int CalcularXp(Reto reto, bool acertado, int tiempoEmpleadoSegundos)
        {
            return acertado ? reto.Dificultad * XpPorDificultad : 0;
        }
    }
}
