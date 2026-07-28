using System;
using System.Collections.Generic;
using AP.Core.Exceptions;
using AP.Data.Entities;
using AP.Repositories;

namespace AP.Core.Business
{
    // SOLID: SRP - esta clase solo contiene las reglas de negocio de las solicitudes de soporte.
    public class SolicitudTIBusiness
    {
        // SOLID: DIP - depende de la abstraccion IRepositorySolicitudTI, no de la implementacion concreta.
        private readonly IRepositorySolicitudTI _repository;

        public SolicitudTIBusiness()
        {
            _repository = new RepositorySolicitudTI();
        }

        // Inyeccion manual para pruebas o cambios futuros sin tocar el controller
        public SolicitudTIBusiness(IRepositorySolicitudTI repository)
        {
            _repository = repository;
        }

        public IEnumerable<SolicitudTI> GetTodas()
        {
            return _repository.GetAll();
        }

        public IEnumerable<SolicitudTI> GetPorUsuario(string usuarioId)
        {
            return _repository.GetPorUsuario(usuarioId);
        }

        public IEnumerable<SolicitudTI> GetPorEstado(EstadoSolicitud estado)
        {
            return _repository.GetPorEstado(estado);
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
            solicitud.CreatedAt = DateTime.Now;

            _repository.Add(solicitud);
            _repository.Save();
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
