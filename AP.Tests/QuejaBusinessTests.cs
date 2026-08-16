using System.Collections.Generic;
using AP.Core.Business;
using AP.Core.Exceptions;
using AP.Data.Entities;
using AP.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AP.Tests
{
    [TestClass]
    public class QuejaBusinessTests
    {
        private class FakeRepositoryQueja : IRepositoryQueja
        {
            public Queja Existente { get; set; }
            public Queja Agregada { get; private set; }
            public Queja Actualizada { get; private set; }

            public IEnumerable<Queja> GetActivas() => new List<Queja>();
            public IEnumerable<Queja> GetPorUsuario(string usuarioId) => new List<Queja>();
            public IEnumerable<Queja> GetAll() => new List<Queja>();
            public Queja GetById(int id) => Existente != null && Existente.QuejaId == id ? Existente : null;
            public void Add(Queja entity) => Agregada = entity;
            public void Update(Queja entity) => Actualizada = entity;
            public void Delete(int id) { }
            public void Save() { }
        }

        private static Queja NuevaQueja(string asunto, string descripcion)
        {
            return new Queja
            {
                UsuarioId = "usuario-1",
                Asunto = asunto,
                Descripcion = descripcion,
                Categoria = CategoriaQueja.Bug
            };
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void Save_AsuntoVacio_LanzaExcepcion(string asunto)
        {
            var repositorio = new FakeRepositoryQueja();
            var business = new QuejaBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() =>
                business.Save(NuevaQueja(asunto, "Descripción de prueba")));
            Assert.IsNull(repositorio.Agregada);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void Save_DescripcionVacia_LanzaExcepcion(string descripcion)
        {
            var repositorio = new FakeRepositoryQueja();
            var business = new QuejaBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() =>
                business.Save(NuevaQueja("Asunto de prueba", descripcion)));
            Assert.IsNull(repositorio.Agregada);
        }

        [TestMethod]
        public void Save_CategoriaFueraDelEnum_LanzaExcepcion()
        {
            var repositorio = new FakeRepositoryQueja();
            var business = new QuejaBusiness(repositorio);
            var queja = NuevaQueja("Asunto de prueba", "Descripción de prueba");
            queja.Categoria = (CategoriaQueja)99;

            Assert.ThrowsExactly<AppException>(() => business.Save(queja));
            Assert.IsNull(repositorio.Agregada);
        }

        [TestMethod]
        public void Save_EstadoDistintoDePendiente_LoFuerzaAPendiente()
        {
            var repositorio = new FakeRepositoryQueja();
            var business = new QuejaBusiness(repositorio);
            var queja = NuevaQueja("Asunto de prueba", "Descripción de prueba");
            queja.Estado = EstadoQueja.Resuelta;

            business.Save(queja);

            Assert.IsNotNull(repositorio.Agregada);
            Assert.AreEqual(EstadoQueja.Pendiente, repositorio.Agregada.Estado);
            Assert.IsTrue(repositorio.Agregada.Activo);
        }

        [TestMethod]
        public void CambiarEstado_IdInexistente_LanzaExcepcion()
        {
            var repositorio = new FakeRepositoryQueja();
            var business = new QuejaBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() =>
                business.CambiarEstado(99, EstadoQueja.Resuelta));
            Assert.IsNull(repositorio.Actualizada);
        }

        [TestMethod]
        public void CambiarEstado_EstadoFueraDelEnum_LanzaExcepcion()
        {
            var repositorio = new FakeRepositoryQueja
            {
                Existente = new Queja { QuejaId = 1, Estado = EstadoQueja.Pendiente }
            };
            var business = new QuejaBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() => business.CambiarEstado(1, (EstadoQueja)99));
            Assert.IsNull(repositorio.Actualizada);
        }

        [TestMethod]
        public void PuedeEditar_AutorConQuejaPendiente_DevuelveTrue()
        {
            var business = new QuejaBusiness(new FakeRepositoryQueja());
            var queja = new Queja { UsuarioId = "usuario-1", Estado = EstadoQueja.Pendiente };

            Assert.IsTrue(business.PuedeEditar(queja, "usuario-1"));
        }

        [TestMethod]
        public void PuedeEditar_OtroUsuario_DevuelveFalse()
        {
            var business = new QuejaBusiness(new FakeRepositoryQueja());
            var queja = new Queja { UsuarioId = "usuario-1", Estado = EstadoQueja.Pendiente };

            Assert.IsFalse(business.PuedeEditar(queja, "usuario-2"));
        }

        [TestMethod]
        [DataRow(EstadoQueja.EnRevision)]
        [DataRow(EstadoQueja.Resuelta)]
        public void PuedeEditar_QuejaYaMovidaPorSoporte_DevuelveFalse(EstadoQueja estado)
        {
            var business = new QuejaBusiness(new FakeRepositoryQueja());
            var queja = new Queja { UsuarioId = "usuario-1", Estado = estado };

            Assert.IsFalse(business.PuedeEditar(queja, "usuario-1"));
        }

        [TestMethod]
        public void Actualizar_QuejaDeOtroUsuario_LanzaExcepcion()
        {
            var repositorio = new FakeRepositoryQueja();
            var business = new QuejaBusiness(repositorio);
            var queja = new Queja
            {
                QuejaId = 1,
                UsuarioId = "usuario-1",
                Asunto = "Asunto de prueba",
                Descripcion = "Descripción de prueba",
                Categoria = CategoriaQueja.Bug,
                Estado = EstadoQueja.Pendiente
            };

            Assert.ThrowsExactly<AppException>(() => business.Actualizar(queja, "usuario-2"));
            Assert.IsNull(repositorio.Actualizada);
        }

        [TestMethod]
        public void Desactivar_AdminSobreQuejaAjena_HaceBorradoLogico()
        {
            var repositorio = new FakeRepositoryQueja
            {
                Existente = new Queja { QuejaId = 1, UsuarioId = "usuario-1", Activo = true }
            };
            var business = new QuejaBusiness(repositorio);

            business.Desactivar(1, "admin-1", true);

            Assert.IsNotNull(repositorio.Actualizada);
            Assert.IsFalse(repositorio.Actualizada.Activo);
        }

        [TestMethod]
        public void Desactivar_OtroUsuarioSinSerAdmin_LanzaExcepcion()
        {
            var repositorio = new FakeRepositoryQueja
            {
                Existente = new Queja { QuejaId = 1, UsuarioId = "usuario-1", Activo = true }
            };
            var business = new QuejaBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() => business.Desactivar(1, "usuario-2", false));
            Assert.IsNull(repositorio.Actualizada);
        }
    }
}
