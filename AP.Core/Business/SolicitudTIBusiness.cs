using System;
using System.Collections.Generic;
using AP.Core.Exceptions;
using AP.Data.Entities;
using AP.Repositories;

namespace AP.Core.Business
{
    public class SolicitudTIBusiness
    {
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
                throw new AppException("La descripci\u00F3n de la solicitud es obligatoria.");

            if (solicitud.FechaCreacion == default(DateTime))
            {
                solicitud.FechaCreacion = DateTime.Now;
            }

            // Avance 3: aqui se completa el flujo de estados y asignacion de agentes.
            solicitud.Estado = EstadoSolicitud.Abierta;
            solicitud.CreatedAt = DateTime.Now;

            _repository.Add(solicitud);
            _repository.Save();
        }
    }
}
