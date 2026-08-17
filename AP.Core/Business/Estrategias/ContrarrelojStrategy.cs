using System;
using AP.Data.Entities;

namespace AP.Core.Business.Estrategias
{
    public class ContrarrelojStrategy : IModoJuegoStrategy
    {
        private readonly ExperienciaBusiness _experiencia = new ExperienciaBusiness();

        public ModoJuego Modo => ModoJuego.Contrarreloj;

        public bool ValidarRespuesta(Reto reto, string respuestaUsuario)
        {
            return string.Equals(
                (respuestaUsuario ?? string.Empty).Trim(),
                reto.RespuestaCorrecta.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }

        public int CalcularXp(Reto reto, bool acertado, int tiempoEmpleadoSegundos)
        {
            if (!acertado)
                return 0;

            // Cuanto antes responda, mas bono suma sobre el XP base.
            int bono = Math.Max(0, (reto.TiempoLimiteSegundos - tiempoEmpleadoSegundos) / 5);
            return _experiencia.CalcularXpGanado(reto.Dificultad, true) + bono;
        }
    }
}
