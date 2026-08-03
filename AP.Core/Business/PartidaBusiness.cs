using System;
using System.Collections.Generic;
using System.Linq;
using AP.Core.Business.Estrategias;
using AP.Core.Exceptions;
using AP.Data;
using AP.Data.Entities;
using AP.Repositories;

namespace AP.Core.Business
{
    public class ResultadoReto
    {
        public bool Acertado { get; set; }
        public string RespuestaCorrecta { get; set; }
        public int XpGanado { get; set; }
        public bool FueraDeTiempo { get; set; }
        public Reto Reto { get; set; }
    }

    // Fila del ranking que expone la capa de negocio; asi la MVC no depende de AP.Repositories.
    public class RankingGlobal
    {
        public string UsuarioId { get; set; }
        public int ExperienciaTotal { get; set; }
        public int PartidasJugadas { get; set; }
        public int Aciertos { get; set; }
    }

    // DP: Strategy - actua como contexto: elige la estrategia del modo y corre el flujo comun sin condicionales.
    public class PartidaBusiness
    {
        private readonly IRepositoryReto _repositoryReto;
        private readonly IRepositoryPartida _repositoryPartida;
        private readonly Dictionary<ModoJuego, IModoJuegoStrategy> _estrategias;
        private static readonly Random _random = new Random();

        public PartidaBusiness()
            : this(new RepositoryReto(), new RepositoryPartida(), EstrategiasPorDefecto())
        {
        }

        public PartidaBusiness(IRepositoryReto repositoryReto, IRepositoryPartida repositoryPartida)
            : this(repositoryReto, repositoryPartida, EstrategiasPorDefecto())
        {
        }

        public PartidaBusiness(MathemaXContext context)
            : this(new RepositoryReto(context), new RepositoryPartida(context), EstrategiasPorDefecto())
        {
        }

        // SOLID: DIP - recibe las abstracciones (repositorios y estrategias), no las crea aca.
        // Inyeccion manual para pruebas o para sumar modos sin tocar el controller.
        public PartidaBusiness(
            IRepositoryReto repositoryReto,
            IRepositoryPartida repositoryPartida,
            IEnumerable<IModoJuegoStrategy> estrategias)
        {
            _repositoryReto = repositoryReto;
            _repositoryPartida = repositoryPartida;
            _estrategias = estrategias.ToDictionary(e => e.Modo);
        }

        public Reto ObtenerRetoAleatorio(ModoJuego modo, int? excluirRetoId = null)
        {
            var retos = _repositoryReto.GetActivosPorModo(modo).ToList();

            if (!retos.Any())
                throw new AppException("No hay retos activos de este modo en este momento.");

            if (excluirRetoId.HasValue && retos.Count > 1)
            {
                var retoAnterior = retos.FirstOrDefault(r => r.RetoId == excluirRetoId.Value);
                var candidatos = retos.Where(r => r.RetoId != excluirRetoId.Value).ToList();

                if (retoAnterior != null)
                {
                    var candidatosDistintos = candidatos
                        .Where(r => !string.Equals(
                            r.Enunciado?.Trim(),
                            retoAnterior.Enunciado?.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (candidatosDistintos.Any())
                        candidatos = candidatosDistintos;
                }

                retos = candidatos;
            }

            var indice = _random.Next(retos.Count);
            return retos[indice];
        }

        public ResultadoReto ResolverReto(
            int retoId,
            string usuarioId,
            string respuestaUsuario,
            int tiempoEmpleadoSegundos,
            ModoJuego? modoEsperado = null)
        {
            var reto = _repositoryReto.GetById(retoId);
            if (reto == null)
                throw new AppException("El reto solicitado ya no existe.");

            if (!reto.Activo)
                throw new AppException("El reto ya no está disponible.");

            if (modoEsperado.HasValue && reto.Modo != modoEsperado.Value)
                throw new AppException("El reto no pertenece al modo solicitado.");

            if (!_estrategias.TryGetValue(reto.Modo, out var estrategia))
                throw new AppException("Modo de juego no soportado.");

            var dentroDeTiempo = tiempoEmpleadoSegundos <= reto.TiempoLimiteSegundos;
            var acertado = dentroDeTiempo && estrategia.ValidarRespuesta(reto, respuestaUsuario);
            var xpGanado = estrategia.CalcularXp(reto, acertado, tiempoEmpleadoSegundos);

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
                FueraDeTiempo = !dentroDeTiempo,
                Reto = reto
            };
        }

        public IEnumerable<Partida> GetHistorial(string usuarioId)
        {
            return _repositoryPartida.GetHistorialPorUsuario(usuarioId);
        }

        public IEnumerable<RankingGlobal> GetRankingGlobal()
        {
            return _repositoryPartida.GetRankingGlobal()
                .Select(r => new RankingGlobal
                {
                    UsuarioId = r.UsuarioId,
                    ExperienciaTotal = r.ExperienciaTotal,
                    PartidasJugadas = r.PartidasJugadas,
                    Aciertos = r.Aciertos
                })
                .ToList();
        }

        // Modos soportados hoy; agregar uno nuevo es sumarlo a esta lista.
        private static IEnumerable<IModoJuegoStrategy> EstrategiasPorDefecto()
        {
            return new IModoJuegoStrategy[]
            {
                new OperadorPerdidoStrategy(),
                new ContrarrelojStrategy(),
                new SecuenciasLogicasStrategy()
            };
        }
    }
}
