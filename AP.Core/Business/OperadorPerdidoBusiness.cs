using System;
using System.Collections.Generic;
using System.Linq;
using AP.Data.Entities;
using AP.Repositories;

namespace AP.Core.Business
{
    public class ResultadoReto
    {
        public bool Acertado { get; set; }
        public string RespuestaCorrecta { get; set; }
        public int XpGanado { get; set; }
        public Reto Reto { get; set; }
    }

    public class OperadorPerdidoBusiness
    {
        private readonly IRepositoryReto _repositoryReto;
        private readonly IRepositoryPartida _repositoryPartida;
        private readonly ExperienciaBusiness _experienciaBusiness;
        private static readonly Random _random= new Random();

        public OperadorPerdidoBusiness()
        {
            _repositoryReto = new RepositoryReto();
            _repositoryPartida= new RepositoryPartida();
            _experienciaBusiness= new ExperienciaBusiness();
        }

        public Reto ObtenerRetoAleatorio()
        {
            var retos = _repositoryReto.GetActivosPorModo(Data.Entities.ModoJuego.OperadorPerdido).ToList();

            if (!retos.Any())
                throw new ApplicationException("No hay retos activos de Operador Perdido en este momento");

            var indice = _random.Next(retos.Count);
            return retos[indice];
        }

        public ResultadoReto ResolverReto(int retoId, string usuarioId, string respuestaUsuario, int tiempoEmpleadoSegundos)
        {
            var reto = _repositoryReto.GetById(retoId);
            if (reto == null)
                throw new ApplicationException("El reto solicitado ya no existe.");

            var dentroDeTiempo = tiempoEmpleadoSegundos <= reto.TiempoLimiteSegundos;
            var respuestaCorrecta = string.Equals(
                (respuestaUsuario ?? string.Empty).Trim(),
                reto.RespuestaCorrecta.Trim(),
                StringComparison.OrdinalIgnoreCase);

            var acertado = respuestaCorrecta && dentroDeTiempo;
            var xpGanado = _experienciaBusiness.CalcularXpGanado(reto.Dificultad, acertado);

            var partida = new Partida
            {
                UsuarioId = usuarioId,
                RetoId = retoId,
                Acertado = acertado,
                TiempoEmpleadoSegundos = tiempoEmpleadoSegundos,
                XpGanado = xpGanado,
                FechaJuego = DateTime.Now
            };

            _repositoryPartida.Add(partida);

            return new ResultadoReto
            {
                Acertado = acertado,
                RespuestaCorrecta = reto.RespuestaCorrecta,
                XpGanado = xpGanado,
                Reto = reto
            };
        }

        public IEnumerable<Partida> GetHistorial(string usuarioId)
        {
            return _repositoryPartida.GetHistorialPorUsuario(usuarioId);
        }
    }
}
