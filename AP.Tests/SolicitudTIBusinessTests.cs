using System.Collections.Generic;
using AP.Core.Business;
using AP.Core.Exceptions;
using AP.Data.Entities;
using AP.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AP.Tests
{
    [TestClass]
    public class SolicitudTIBusinessTests
    {
        private class FakeRepositorySolicitudTI : IRepositorySolicitudTI
        {
            public SolicitudTI Existente { get; set; }
            public SolicitudTI Agregada { get; private set; }
            public SolicitudTI Actualizada { get; private set; }

            public IEnumerable<SolicitudTI> GetActivas(int pagina, int tamanoPagina) => new List<SolicitudTI>();
            public int ContarActivas() => 0;
            public IEnumerable<SolicitudTI> GetPorUsuario(string usuarioId, int pagina, int tamanoPagina) => new List<SolicitudTI>();
            public int ContarPorUsuario(string usuarioId) => 0;
            public SolicitudTI GetById(int id) => Existente != null && Existente.SolicitudTIId == id ? Existente : null;
            public void Add(SolicitudTI entity) => Agregada = entity;
            public void Update(SolicitudTI entity) => Actualizada = entity;
        }

        private static SolicitudTI NuevaSolicitud(string asunto, string descripcion)
        {
            return new SolicitudTI
            {
                UsuarioId = "usuario-1",
                Asunto = asunto,
                Descripcion = descripcion
            };
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void Save_AsuntoVacio_LanzaExcepcion(string asunto)
        {
            var repositorio = new FakeRepositorySolicitudTI();
            var business = new SolicitudTIBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() =>
                business.Save(NuevaSolicitud(asunto, "Descripción de prueba")));
            Assert.IsNull(repositorio.Agregada);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void Save_DescripcionVacia_LanzaExcepcion(string descripcion)
        {
            var repositorio = new FakeRepositorySolicitudTI();
            var business = new SolicitudTIBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() =>
                business.Save(NuevaSolicitud("Asunto de prueba", descripcion)));
            Assert.IsNull(repositorio.Agregada);
        }

        [TestMethod]
        public void Save_SolicitudNula_LanzaExcepcion()
        {
            var repositorio = new FakeRepositorySolicitudTI();
            var business = new SolicitudTIBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() => business.Save(null));
            Assert.IsNull(repositorio.Agregada);
        }

        [TestMethod]
        public void Save_EstadoDistintoDeAbierta_LoFuerzaAAbierta()
        {
            var repositorio = new FakeRepositorySolicitudTI();
            var business = new SolicitudTIBusiness(repositorio);
            var solicitud = NuevaSolicitud("Asunto de prueba", "Descripción de prueba");
            solicitud.Estado = EstadoSolicitud.Cerrada;

            business.Save(solicitud);

            Assert.IsNotNull(repositorio.Agregada);
            Assert.AreEqual(EstadoSolicitud.Abierta, repositorio.Agregada.Estado);
            Assert.IsTrue(repositorio.Agregada.Activo);
        }

        [TestMethod]
        public void CambiarEstado_IdInexistente_LanzaExcepcion()
        {
            var repositorio = new FakeRepositorySolicitudTI();
            var business = new SolicitudTIBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() =>
                business.CambiarEstado(99, EstadoSolicitud.Cerrada));
            Assert.IsNull(repositorio.Actualizada);
        }

        [TestMethod]
        public void CambiarEstado_EstadoFueraDelEnum_LanzaExcepcion()
        {
            var repositorio = new FakeRepositorySolicitudTI
            {
                Existente = new SolicitudTI { SolicitudTIId = 1, Estado = EstadoSolicitud.Abierta }
            };
            var business = new SolicitudTIBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() => business.CambiarEstado(1, (EstadoSolicitud)99));
            Assert.IsNull(repositorio.Actualizada);
        }

        [TestMethod]
        public void PuedeEditar_AutorConSolicitudAbierta_DevuelveTrue()
        {
            var business = new SolicitudTIBusiness(new FakeRepositorySolicitudTI());
            var solicitud = new SolicitudTI { UsuarioId = "usuario-1", Estado = EstadoSolicitud.Abierta };

            Assert.IsTrue(business.PuedeEditar(solicitud, "usuario-1"));
        }

        [TestMethod]
        public void PuedeEditar_OtroUsuario_DevuelveFalse()
        {
            var business = new SolicitudTIBusiness(new FakeRepositorySolicitudTI());
            var solicitud = new SolicitudTI { UsuarioId = "usuario-1", Estado = EstadoSolicitud.Abierta };

            Assert.IsFalse(business.PuedeEditar(solicitud, "usuario-2"));
        }

        [TestMethod]
        [DataRow(EstadoSolicitud.EnProceso)]
        [DataRow(EstadoSolicitud.Cerrada)]
        public void PuedeEditar_SolicitudYaTomadaPorSoporte_DevuelveFalse(EstadoSolicitud estado)
        {
            var business = new SolicitudTIBusiness(new FakeRepositorySolicitudTI());
            var solicitud = new SolicitudTI { UsuarioId = "usuario-1", Estado = estado };

            Assert.IsFalse(business.PuedeEditar(solicitud, "usuario-1"));
        }

        [TestMethod]
        public void Actualizar_SolicitudDeOtroUsuario_LanzaExcepcion()
        {
            var repositorio = new FakeRepositorySolicitudTI();
            var business = new SolicitudTIBusiness(repositorio);
            var solicitud = new SolicitudTI
            {
                SolicitudTIId = 1,
                UsuarioId = "usuario-1",
                Asunto = "Asunto de prueba",
                Descripcion = "Descripción de prueba",
                Estado = EstadoSolicitud.Abierta
            };

            Assert.ThrowsExactly<AppException>(() => business.Actualizar(solicitud, "usuario-2"));
            Assert.IsNull(repositorio.Actualizada);
        }

        [TestMethod]
        public void Desactivar_AdminSobreSolicitudAjena_HaceBorradoLogico()
        {
            var repositorio = new FakeRepositorySolicitudTI
            {
                Existente = new SolicitudTI { SolicitudTIId = 1, UsuarioId = "usuario-1", Activo = true }
            };
            var business = new SolicitudTIBusiness(repositorio);

            business.Desactivar(1, "admin-1", true);

            Assert.IsNotNull(repositorio.Actualizada);
            Assert.IsFalse(repositorio.Actualizada.Activo);
        }

        [TestMethod]
        public void Desactivar_OtroUsuarioSinSerAdmin_LanzaExcepcion()
        {
            var repositorio = new FakeRepositorySolicitudTI
            {
                Existente = new SolicitudTI { SolicitudTIId = 1, UsuarioId = "usuario-1", Activo = true }
            };
            var business = new SolicitudTIBusiness(repositorio);

            Assert.ThrowsExactly<AppException>(() => business.Desactivar(1, "usuario-2", false));
            Assert.IsNull(repositorio.Actualizada);
        }
    }
}
