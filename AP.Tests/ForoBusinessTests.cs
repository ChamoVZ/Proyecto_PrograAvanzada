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
            public int TotalActivas { get; set; }
            public int PaginaSolicitada { get; private set; }
            public int TamanoSolicitado { get; private set; }

            public IEnumerable<Publicacion> GetActivasRecientes(int pagina, int tamanoPagina)
            {
                PaginaSolicitada = pagina;
                TamanoSolicitado = tamanoPagina;
                return new List<Publicacion>();
            }
            public int ContarActivas() => TotalActivas;
            public Publicacion GetById(int id) => Existente != null && Existente.PublicacionId == id ? Existente : null;
            public void Add(Publicacion entity) => Agregada = entity;
            public void Update(Publicacion entity) => Actualizada = entity;
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

        [TestMethod]
        public void GetActivasRecientes_PaginaFueraDeRango_UsaLaUltima()
        {
            var repositorio = new FakeRepositoryPublicacion { TotalActivas = 21 };
            var business = new ForoBusiness(repositorio);

            var resultado = business.GetActivasRecientes(9, 10);

            Assert.AreEqual(3, resultado.PaginaActual);
            Assert.AreEqual(3, repositorio.PaginaSolicitada);
        }

        [TestMethod]
        public void GetActivasRecientes_TamanoInvalido_UsaDiez()
        {
            var repositorio = new FakeRepositoryPublicacion { TotalActivas = 25 };
            var business = new ForoBusiness(repositorio);

            var resultado = business.GetActivasRecientes(1, 15);

            Assert.AreEqual(10, resultado.TamanoPagina);
            Assert.AreEqual(10, repositorio.TamanoSolicitado);
        }

        [TestMethod]
        public void GetActivasRecientes_TamanoVeinte_UsaVeinte()
        {
            var repositorio = new FakeRepositoryPublicacion { TotalActivas = 25 };
            var business = new ForoBusiness(repositorio);

            var resultado = business.GetActivasRecientes(1, 20);

            Assert.AreEqual(20, resultado.TamanoPagina);
            Assert.AreEqual(20, repositorio.TamanoSolicitado);
        }

        [TestMethod]
        [DataRow(20, 2)]
        [DataRow(21, 3)]
        public void GetActivasRecientes_CalculaTotalPaginas(int totalRegistros, int totalPaginas)
        {
            var repositorio = new FakeRepositoryPublicacion { TotalActivas = totalRegistros };
            var business = new ForoBusiness(repositorio);

            var resultado = business.GetActivasRecientes(1, 10);

            Assert.AreEqual(totalPaginas, resultado.TotalPaginas);
        }
    }
}
