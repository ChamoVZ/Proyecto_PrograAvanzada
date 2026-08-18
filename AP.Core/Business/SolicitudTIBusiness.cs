using System;
using System.Collections.Generic;
using AP.Core.Exceptions;
using AP.Data;
using AP.Data.Entities;
using AP.Repositories;

namespace AP.Core.Business
{
    // SOLID: SRP - esta clase solo contiene las reglas de negocio de las solicitudes de soporte.
    public class SolicitudTIBusiness
    {
        // SOLID: DIP - depende de la abstraccion IRepositorySolicitudTI, no de la implementacion concreta.
        private readonly IRepositorySolicitudTI _repository;

        // Para compartir el contexto con otro Business dentro de la misma transaccion.
        public SolicitudTIBusiness(MathemaXContext context)
        {
            _repository = new RepositorySolicitudTI(context);
        }

        // Inyeccion manual para pruebas o cambios futuros sin tocar el controller
        public SolicitudTIBusiness(IRepositorySolicitudTI repository)
        {
            _repository = repository;
        }

        public ResultadoPaginado<SolicitudTI> GetTodas(int pagina, int tamanoPagina)
        {
            var total = _repository.ContarActivas();
            return ResultadoPaginado<SolicitudTI>.Crear(
                pagina,
                tamanoPagina,
                total,
                _repository.GetActivas);
        }

        public SolicitudTI GetPorId(int id)
        {
            return _repository.GetById(id);
        }

        public ResultadoPaginado<SolicitudTI> GetPorUsuario(string usuarioId, int pagina, int tamanoPagina)
        {
            var total = _repository.ContarPorUsuario(usuarioId);
            return ResultadoPaginado<SolicitudTI>.Crear(
                pagina,
                tamanoPagina,
                total,
                (paginaActual, tamanoActual) =>
                    _repository.GetPorUsuario(usuarioId, paginaActual, tamanoActual));
        }

        public void Save(SolicitudTI solicitud)
        {
            if (solicitud == null)
                throw new AppException("La solicitud no puede ser nula.");

            if (string.IsNullOrWhiteSpace(solicitud.Asunto))
                throw new AppException("El asunto de la solicitud es obligatorio.");

            if (string.IsNullOrWhiteSpace(solicitud.Descripcion))
                throw new AppException("La descripción de la solicitud es obligatoria.");

            if (solicitud.FechaCreacion == default(DateTime))
            {
                solicitud.FechaCreacion = DateTime.Now;
            }

            // Toda solicitud nueva entra Abierta; el soporte la avanza con CambiarEstado.
            solicitud.Estado = EstadoSolicitud.Abierta;
            solicitud.Activo = true;
            solicitud.CreatedAt = DateTime.Now;

            _repository.Add(solicitud);
        }

        // Quien puede editar y quien puede eliminar se decide aca; el controller y la vista
        // solo consultan el resultado.
        public bool PuedeEditar(SolicitudTI solicitud, string usuarioId)
        {
            // Una vez que soporte la movio de Abierta, el autor ya no la toca.
            return solicitud != null
                && solicitud.UsuarioId == usuarioId
                && solicitud.Estado == EstadoSolicitud.Abierta;
        }

        public bool PuedeEliminar(SolicitudTI solicitud, string usuarioId, bool esAdmin)
        {
            return solicitud != null && (esAdmin || solicitud.UsuarioId == usuarioId);
        }

        public void Actualizar(SolicitudTI solicitud, string usuarioId)
        {
            if (solicitud == null)
                throw new AppException("La solicitud no puede ser nula.");

            if (!PuedeEditar(solicitud, usuarioId))
                throw new AppException("Solo el autor puede editar su solicitud, y solo mientras siga abierta.");

            if (string.IsNullOrWhiteSpace(solicitud.Asunto))
                throw new AppException("El asunto de la solicitud es obligatorio.");

            if (string.IsNullOrWhiteSpace(solicitud.Descripcion))
                throw new AppException("La descripción de la solicitud es obligatoria.");

            solicitud.LastModified = DateTime.Now;
            _repository.Update(solicitud);
        }

        public void Desactivar(int id, string usuarioId, bool esAdmin)
        {
            var solicitud = _repository.GetById(id);
            if (solicitud == null)
                throw new AppException("La solicitud que intenta eliminar no existe.");

            if (!PuedeEliminar(solicitud, usuarioId, esAdmin))
                throw new AppException("Solo el autor o un administrador pueden eliminar esta solicitud.");

            // Borrado logico: el registro se conserva pero deja de listarse en soporte.
            solicitud.Activo = false;
            solicitud.LastModified = DateTime.Now;
            _repository.Update(solicitud);
        }

        public void CambiarEstado(int id, EstadoSolicitud nuevoEstado)
        {
            if (!Enum.IsDefined(typeof(EstadoSolicitud), nuevoEstado))
                throw new AppException("El estado seleccionado no es válido.");

            var solicitud = _repository.GetById(id);
            if (solicitud == null)
                throw new AppException("La solicitud que intenta actualizar no existe.");

            solicitud.Estado = nuevoEstado;
            solicitud.LastModified = DateTime.Now;
            _repository.Update(solicitud);
        }
    }
}
