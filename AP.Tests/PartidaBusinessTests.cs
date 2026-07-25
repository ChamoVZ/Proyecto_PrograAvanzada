using System.Collections.Generic;
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
        private class FakeRepositoryReto : IRepositoryReto
        {
            public Reto Reto { get; set; }

            public IEnumerable<Reto> GetActivosPorModo(ModoJuego modo) => new List<Reto>();
            public IEnumerable<Reto> GetAll() => new List<Reto>();
            public Reto GetById(int id) => Reto != null && Reto.RetoId == id ? Reto : null;
            public void Add(Reto entity) { }
            public void Update(Reto entity) { }
            public void Delete(int id) { }
            public void Save() { }
        }

        private class FakeRepositoryPartida : IRepositoryPartida
        {
            public Partida Agregada { get; private set; }

            public IEnumerable<Partida> GetHistorialPorUsuario(string usuarioId) => new List<Partida>();
            public IEnumerable<RankingUsuario> GetRankingGlobal() => new List<RankingUsuario>();
            public IEnumerable<Partida> GetAll() => new List<Partida>();
            public Partida GetById(int id) => null;
            public void Add(Partida entity) => Agregada = entity;
            public void Update(Partida entity) { }
            public void Delete(int id) { }
            public void Save() { }
        }

        private static PartidaBusiness CrearBusiness(Reto reto, FakeRepositoryPartida partidas)
        {
            return new PartidaBusiness(
                new FakeRepositoryReto { Reto = reto },
                partidas,
                new IModoJuegoStrategy[] { new ContrarrelojStrategy() });
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
    }
}
