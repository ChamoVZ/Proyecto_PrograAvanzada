using System.Collections.Generic;
using AP.Core.Business;
using AP.Core.Exceptions;
using AP.Data.Entities;
using AP.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AP.Tests
{
    [TestClass]
    public class RetoBusinessTests
    {
        // Repositorio falso: evita tocar la base de datos y deja verificar que se llamo al metodo correcto.
        private class FakeRepositoryReto : IRepositoryReto
        {
            public Reto Agregado;
            public Reto Actualizado;

            public IEnumerable<Reto> GetActivosPorModo(ModoJuego modo) => new List<Reto>();
            public IEnumerable<Reto> GetPagina(int pagina, int tamanoPagina) => new List<Reto>();
            public int Contar() => 0;
            public IEnumerable<Reto> GetAll() => new List<Reto>();
            public Reto GetById(int id) => null;
            public void Add(Reto entity) => Agregado = entity;
            public void Update(Reto entity) => Actualizado = entity;
            public void Save() { }
        }

        private static Reto NuevoReto(int id, int dificultad, int tiempoLimite)
        {
            return new Reto
            {
                RetoId = id,
                Titulo = "Reto de prueba",
                Modo = ModoJuego.OperadorPerdido,
                Enunciado = "4 _ 3 = 12",
                RespuestaCorrecta = "*",
                Dificultad = dificultad,
                TiempoLimiteSegundos = tiempoLimite
            };
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(6)]
        public void SaveOrUpdate_DificultadInvalida_LanzaExcepcion(int dificultad)
        {
            var business = new RetoBusiness(new FakeRepositoryReto());
            var reto = NuevoReto(0, dificultad, 30);

            Assert.ThrowsExactly<AppException>(() => business.SaveOrUpdate(reto));
        }

        [TestMethod]
        public void SaveOrUpdate_TiempoLimiteCero_LanzaExcepcion()
        {
            var business = new RetoBusiness(new FakeRepositoryReto());
            var reto = NuevoReto(0, 3, 0);

            Assert.ThrowsExactly<AppException>(() => business.SaveOrUpdate(reto));
        }

        [TestMethod]
        public void SaveOrUpdate_RetoNuevo_LoAgregaYLoMarcaActivo()
        {
            var repositorio = new FakeRepositoryReto();
            var business = new RetoBusiness(repositorio);
            var reto = NuevoReto(0, 3, 30);

            business.SaveOrUpdate(reto);

            Assert.IsNotNull(repositorio.Agregado);
            Assert.IsTrue(repositorio.Agregado.Activo);
            Assert.IsNull(repositorio.Actualizado);
        }

        [TestMethod]
        public void SaveOrUpdate_RetoExistente_LoActualiza()
        {
            var repositorio = new FakeRepositoryReto();
            var business = new RetoBusiness(repositorio);
            var reto = NuevoReto(5, 3, 30);

            business.SaveOrUpdate(reto);

            Assert.IsNotNull(repositorio.Actualizado);
            Assert.IsNull(repositorio.Agregado);
        }
    }
}
