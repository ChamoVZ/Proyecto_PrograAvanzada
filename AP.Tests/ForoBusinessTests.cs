using System;
using System.Collections.Generic;
using AP.Core.Business;
using AP.Core.Exceptions;
using AP.Data.Entities;
using AP.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AP.Tests
{
    [TestClass]
    public class ForoBusinessTests
    {
        private class FakeRepositoryPublicacion : IRepositoryPublicacion
        {
            public Publicacion Existente { get; set; }
            public Publicacion Agregada { get; private set; }
            public Publicacion Actualizada { get; private set; }

            public IEnumerable<Publicacion> GetActivasRecientes() => new List<Publicacion>();
            public IEnumerable<Publicacion> GetAll() => new List<Publicacion>();
            public Publicacion GetById(int id) => Existente != null && Existente.PublicacionId == id ? Existente : null;
            public void Add(Publicacion entity) => Agregada = entity;
            public void Update(Publicacion entity) => Actualizada = entity;
            public void Delete(int id) { }
            public void Save() { }
        }

        private static Publicacion NuevaPublicacion(string titulo, string contenido)
        {
            return new Publicacion
            {
                UsuarioId = "usuario-1",
                Titulo = titulo,
                Contenido = contenido
            };
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void Save_TituloVacio_LanzaExcepcion(string titulo)
        {
            var repositorio = new FakeRepositoryPublicacion();
            var business = new ForoBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() =>
                business.Save(NuevaPublicacion(titulo, "Contenido de prueba")));
            Assert.IsNull(repositorio.Agregada);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void Save_ContenidoVacio_LanzaExcepcion(string contenido)
        {
            var repositorio = new FakeRepositoryPublicacion();
            var business = new ForoBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() =>
                business.Save(NuevaPublicacion("Título de prueba", contenido)));
            Assert.IsNull(repositorio.Agregada);
        }

        [TestMethod]
        public void Save_PublicacionNula_LanzaExcepcion()
        {
            var repositorio = new FakeRepositoryPublicacion();
            var business = new ForoBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() => business.Save(null));
            Assert.IsNull(repositorio.Agregada);
        }

        [TestMethod]
        public void Save_PublicacionValida_LaMarcaActivaYSellaLaFecha()
        {
            var repositorio = new FakeRepositoryPublicacion();
            var business = new ForoBusiness(repositorio);
            var publicacion = NuevaPublicacion("Título de prueba", "Contenido de prueba");

            business.Save(publicacion);

            Assert.IsNotNull(repositorio.Agregada);
            Assert.IsTrue(repositorio.Agregada.Activo);
            Assert.AreNotEqual(default(DateTime), repositorio.Agregada.CreatedAt);
            Assert.AreNotEqual(default(DateTime), repositorio.Agregada.FechaPublicacion);
        }

        [TestMethod]
        public void Desactivar_IdInexistente_LanzaExcepcion()
        {
            var repositorio = new FakeRepositoryPublicacion();
            var business = new ForoBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() => business.Desactivar(99, "usuario-1", false));
            Assert.IsNull(repositorio.Actualizada);
        }

        [TestMethod]
        public void Desactivar_AutorSobreLoSuyo_HaceBorradoLogico()
        {
            var repositorio = new FakeRepositoryPublicacion
            {
                Existente = new Publicacion { PublicacionId = 7, UsuarioId = "usuario-1", Activo = true }
            };
            var business = new ForoBusiness(repositorio);

            business.Desactivar(7, "usuario-1", false);

            Assert.IsNotNull(repositorio.Actualizada);
            Assert.IsFalse(repositorio.Actualizada.Activo);
        }

        [TestMethod]
        public void PuedeModificar_Autor_DevuelveTrue()
        {
            var business = new ForoBusiness(new FakeRepositoryPublicacion());
            var publicacion = new Publicacion { UsuarioId = "usuario-1" };

            Assert.IsTrue(business.PuedeModificar(publicacion, "usuario-1", false));
        }

        [TestMethod]
        public void PuedeModificar_AdminSobrePublicacionAjena_DevuelveTrue()
        {
            var business = new ForoBusiness(new FakeRepositoryPublicacion());
            var publicacion = new Publicacion { UsuarioId = "usuario-1" };

            Assert.IsTrue(business.PuedeModificar(publicacion, "admin-1", true));
        }

        [TestMethod]
        public void PuedeModificar_OtroUsuarioSinSerAdmin_DevuelveFalse()
        {
            var business = new ForoBusiness(new FakeRepositoryPublicacion());
            var publicacion = new Publicacion { UsuarioId = "usuario-1" };

            Assert.IsFalse(business.PuedeModificar(publicacion, "usuario-2", false));
        }

        [TestMethod]
        public void Actualizar_PublicacionDeOtroUsuario_LanzaExcepcion()
        {
            var repositorio = new FakeRepositoryPublicacion();
            var business = new ForoBusiness(repositorio);
            var publicacion = new Publicacion
            {
                PublicacionId = 7,
                UsuarioId = "usuario-1",
                Titulo = "Título de prueba",
                Contenido = "Contenido de prueba"
            };

            Assert.ThrowsExactly<AppException>(() => business.Actualizar(publicacion, "usuario-2", false));
            Assert.IsNull(repositorio.Actualizada);
        }

        [TestMethod]
        public void Actualizar_AdminSobrePublicacionAjena_LaGuarda()
        {
            var repositorio = new FakeRepositoryPublicacion();
            var business = new ForoBusiness(repositorio);
            var publicacion = new Publicacion
            {
                PublicacionId = 7,
                UsuarioId = "usuario-1",
                Titulo = "Título moderado",
                Contenido = "Contenido moderado"
            };

            business.Actualizar(publicacion, "admin-1", true);

            Assert.IsNotNull(repositorio.Actualizada);
            Assert.AreEqual("Título moderado", repositorio.Actualizada.Titulo);
        }

        [TestMethod]
        public void Desactivar_OtroUsuarioSinSerAdmin_LanzaExcepcion()
        {
            var repositorio = new FakeRepositoryPublicacion
            {
                Existente = new Publicacion { PublicacionId = 7, UsuarioId = "usuario-1", Activo = true }
            };
            var business = new ForoBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() => business.Desactivar(7, "usuario-2", false));
            Assert.IsNull(repositorio.Actualizada);
        }
    }
}
