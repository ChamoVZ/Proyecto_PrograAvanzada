using System;
using AP.Data.Entities;

namespace AP.Core.Business.Estrategias
{
    // SOLID: OCP - agregar un modo nuevo es crear otra estrategia, sin tocar las existentes.
    public class OperadorPerdidoStrategy : IModoJuegoStrategy
    {
        private readonly ExperienciaBusiness _experiencia = new ExperienciaBusiness();

        public ModoJuego Modo => ModoJuego.OperadorPerdido;

        public bool ValidarRespuesta(Reto reto, string respuestaUsuario)
        {
            return string.Equals(
                (respuestaUsuario ?? string.Empty).Trim(),
                reto.RespuestaCorrecta.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }

        public int CalcularXp(Reto reto, bool acertado, int tiempoEmpleadoSegundos)
        {
            // El XP base ya vive en ExperienciaBusiness, no lo repetimos aqui.
            return _experiencia.CalcularXpGanado(reto.Dificultad, acertado);
        }
    }
}
