using System.Collections.Generic;
using System.Linq;
using AP.Core.Business;
using AP.Core.Business.Estrategias;
using AP.Core.Exceptions;
using AP.Data.Entities;
using AP.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AP.Tests
{
    [TestClass]
    public class PartidaBusinessTests
    {
        private class RegistroHistorial
        {
            public string UsuarioId { get; set; }
            public HistorialUsuario Datos { get; set; }
        }

        private class FakeRepositoryReto : IRepositoryReto
        {
            public Reto Reto { get; set; }

            public IEnumerable<Reto> GetActivosPorModo(ModoJuego modo) => new List<Reto>();
            public IEnumerable<Reto> GetPagina(int pagina, int tamanoPagina) => new List<Reto>();
            public int Contar() => 0;
            public Reto GetById(int id) => Reto != null && Reto.RetoId == id ? Reto : null;
            public void Add(Reto entity) { }
            public void Update(Reto entity) { }
        }

        private class FakeRepositoryPartida : IRepositoryPartida
        {
            public Partida Agregada { get; private set; }
            public List<RegistroHistorial> Historial { get; } = new List<RegistroHistorial>();

            public IEnumerable<HistorialUsuario> GetHistorialPorUsuario(
                string usuarioId,
                bool? resultado,
                ModoJuego? modo,
                System.DateTime? desde,
                System.DateTime? hasta,
                int pagina,
                int tamanoPagina)
            {
                return FiltrarHistorial(usuarioId, resultado, modo, desde, hasta)
                    .OrderByDescending(h => h.FechaJuego)
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList();
            }
            public int ContarHistorialPorUsuario(
                string usuarioId,
                bool? resultado,
                ModoJuego? modo,
                System.DateTime? desde,
                System.DateTime? hasta)
            {
                return FiltrarHistorial(usuarioId, resultado, modo, desde, hasta).Count();
            }
            public IEnumerable<RankingUsuario> GetRankingGlobal(int pagina, int tamanoPagina) => new List<RankingUsuario>();
            public int ContarRankingGlobal() => 0;
            public Partida GetById(int id) => null;
            public void Add(Partida entity) => Agregada = entity;
            public void Update(Partida entity) { }

            private IEnumerable<HistorialUsuario> FiltrarHistorial(
                string usuarioId,
                bool? resultado,
                ModoJuego? modo,
                System.DateTime? desde,
                System.DateTime? hasta)
            {
                var query = Historial.Where(h => h.UsuarioId == usuarioId);

                if (resultado.HasValue)
                {
                    query = query.Where(h => h.Datos.Acertado == resultado.Value);
                }

                if (modo.HasValue)
                {
                    query = query.Where(h => h.Datos.Modo == modo.Value);
                }

                if (desde.HasValue)
                {
                    query = query.Where(h => h.Datos.FechaJuego >= desde.Value.Date);
                }

                if (hasta.HasValue)
                {
                    var hastaExclusiva = hasta.Value.Date.AddDays(1);
                    query = query.Where(h => h.Datos.FechaJuego < hastaExclusiva);
                }

                return query.Select(h => h.Datos);
            }
        }

        private static PartidaBusiness CrearBusiness(Reto reto, FakeRepositoryPartida partidas)
        {
            return new PartidaBusiness(
                new FakeRepositoryReto { Reto = reto },
                partidas,
                new IModoJuegoStrategy[] { new ContrarrelojStrategy() });
        }

        private static PartidaBusiness CrearBusinessHistorial(FakeRepositoryPartida partidas)
        {
            return new PartidaBusiness(new FakeRepositoryReto(), partidas);
        }

        private static RegistroHistorial NuevoRegistro(
            string usuarioId,
            bool acertado,
            ModoJuego modo,
            System.DateTime fecha,
            string titulo)
        {
            return new RegistroHistorial
            {
                UsuarioId = usuarioId,
                Datos = new HistorialUsuario
                {
                    FechaJuego = fecha,
                    Acertado = acertado,
                    Modo = modo,
                    TituloReto = titulo
                }
            };
        }

        [TestMethod]
        public void ResolverReto_ContrarrelojCorrecto_RegistraPartidaConBono()
        {
            var reto = new Reto
            {
                RetoId = 10,
                Modo = ModoJuego.Contrarreloj,
                RespuestaCorrecta = "83",
                Dificultad = 2,
                TiempoLimiteSegundos = 20,
                Activo = true
            };
            var partidas = new FakeRepositoryPartida();
            var business = CrearBusiness(reto, partidas);

            var resultado = business.ResolverReto(10, "usuario-1", "83", 10, ModoJuego.Contrarreloj);

            Assert.IsTrue(resultado.Acertado);
            Assert.AreEqual(22, resultado.XpGanado);
            Assert.IsNotNull(partidas.Agregada);
            Assert.AreEqual(22, partidas.Agregada.XpGanado);
        }

        [TestMethod]
        public void ResolverReto_RetoInactivo_NoRegistraPartida()
        {
            var reto = new Reto
            {
                RetoId = 10,
                Modo = ModoJuego.Contrarreloj,
                RespuestaCorrecta = "83",
                Dificultad = 2,
                TiempoLimiteSegundos = 20,
                Activo = false
            };
            var partidas = new FakeRepositoryPartida();
            var business = CrearBusiness(reto, partidas);

            Assert.ThrowsExactly<AppException>(() =>
                business.ResolverReto(10, "usuario-1", "83", 10, ModoJuego.Contrarreloj));
            Assert.IsNull(partidas.Agregada);
        }

        [TestMethod]
        public void ResolverReto_ModoDistinto_NoRegistraPartida()
        {
            var reto = new Reto
            {
                RetoId = 10,
                Modo = ModoJuego.OperadorPerdido,
                RespuestaCorrecta = "*",
                Dificultad = 2,
                TiempoLimiteSegundos = 20,
                Activo = true
            };
            var partidas = new FakeRepositoryPartida();
            var business = CrearBusiness(reto, partidas);

            Assert.ThrowsExactly<AppException>(() =>
                business.ResolverReto(10, "usuario-1", "*", 10, ModoJuego.Contrarreloj));
            Assert.IsNull(partidas.Agregada);
        }

        [TestMethod]
        public void GetHistorial_FiltroPorResultado_DevuelveSoloAcertadas()
        {
            var repositorio = new FakeRepositoryPartida();
            repositorio.Historial.Add(NuevoRegistro("usuario-1", true, ModoJuego.OperadorPerdido, System.DateTime.Today, "Acierto"));
            repositorio.Historial.Add(NuevoRegistro("usuario-1", false, ModoJuego.OperadorPerdido, System.DateTime.Today, "Fallo"));
            var business = CrearBusinessHistorial(repositorio);

            var resultado = business.GetHistorial("usuario-1", true, null, null, null, 1, 10);

            Assert.AreEqual(1, resultado.TotalRegistros);
            Assert.IsTrue(resultado.Elementos.All(h => h.Acertado));
        }

        [TestMethod]
        public void GetHistorial_FiltroPorModo_DevuelveSoloElModoElegido()
        {
            var repositorio = new FakeRepositoryPartida();
            repositorio.Historial.Add(NuevoRegistro("usuario-1", true, ModoJuego.Contrarreloj, System.DateTime.Today, "Contrarreloj"));
            repositorio.Historial.Add(NuevoRegistro("usuario-1", true, ModoJuego.SecuenciasLogicas, System.DateTime.Today, "Secuencia"));
            var business = CrearBusinessHistorial(repositorio);

            var resultado = business.GetHistorial(
                "usuario-1",
                null,
                ModoJuego.SecuenciasLogicas,
                null,
                null,
                1,
                10);

            Assert.AreEqual(1, resultado.TotalRegistros);
            Assert.AreEqual(ModoJuego.SecuenciasLogicas, resultado.Elementos.Single().Modo);
        }

        [TestMethod]
        public void GetHistorial_RangoInvertido_LanzaExcepcion()
        {
            var business = CrearBusinessHistorial(new FakeRepositoryPartida());

            Assert.ThrowsExactly<AppException>(() => business.GetHistorial(
                "usuario-1",
                null,
                null,
                new System.DateTime(2026, 8, 18),
                new System.DateTime(2026, 8, 17),
                1,
                10));
        }

        [TestMethod]
        public void GetHistorial_FechaHasta_IncluyeTodoElDia()
        {
            var repositorio = new FakeRepositoryPartida();
            repositorio.Historial.Add(NuevoRegistro(
                "usuario-1",
                true,
                ModoJuego.Contrarreloj,
                new System.DateTime(2026, 8, 17, 23, 59, 59),
                "Dentro"));
            repositorio.Historial.Add(NuevoRegistro(
                "usuario-1",
                true,
                ModoJuego.Contrarreloj,
                new System.DateTime(2026, 8, 18, 0, 0, 0),
                "Fuera"));
            var business = CrearBusinessHistorial(repositorio);

            var resultado = business.GetHistorial(
                "usuario-1",
                null,
                null,
                null,
                new System.DateTime(2026, 8, 17),
                1,
                10);

            Assert.AreEqual(1, resultado.TotalRegistros);
            Assert.AreEqual("Dentro", resultado.Elementos.Single().TituloReto);
        }

        [TestMethod]
        public void GetHistorial_FiltroNoIncluyePartidasDeOtroUsuario()
        {
            var repositorio = new FakeRepositoryPartida();
            repositorio.Historial.Add(NuevoRegistro("usuario-1", true, ModoJuego.OperadorPerdido, System.DateTime.Today, "Propia"));
            repositorio.Historial.Add(NuevoRegistro("usuario-2", true, ModoJuego.OperadorPerdido, System.DateTime.Today, "Ajena"));
            var business = CrearBusinessHistorial(repositorio);

            var resultado = business.GetHistorial(
                "usuario-1",
                true,
                ModoJuego.OperadorPerdido,
                null,
                null,
                1,
                10);

            Assert.AreEqual(1, resultado.TotalRegistros);
            Assert.AreEqual("Propia", resultado.Elementos.Single().TituloReto);
        }
    }
}
