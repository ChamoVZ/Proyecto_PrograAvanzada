using System;
using System.Collections.Generic;
using AP.Data.Entities;
using AP.Repositories;
using AP.Core.Exceptions;

namespace AP.Core.Business
{
    public class ForoBusiness
    {
        private readonly IRepositoryPublicacion _repository;

        public ForoBusiness()
        {
            _repository = new RepositoryPublicacion();
        }

        // Inyección de dependencias manual (o por framework en un futuro)
        public ForoBusiness(IRepositoryPublicacion repository)
        {
            _repository = repository;
        }

        public IEnumerable<Publicacion> GetActivasRecientes()
        {
            return _repository.GetActivasRecientes();
        }

        public void Save(Publicacion publicacion)
        {
            if (publicacion == null)
                throw new AppException("La publicación no puede ser nula.");

            if (string.IsNullOrWhiteSpace(publicacion.Titulo))
                throw new AppException("El título de la publicación es obligatorio.");

            if (string.IsNullOrWhiteSpace(publicacion.Contenido))
                throw new AppException("El contenido de la publicación no puede estar vacío.");

            // Aseguramos que la fecha de publicación sea la actual si no se provee
            if (publicacion.FechaPublicacion == default(DateTime))
            {
                publicacion.FechaPublicacion = DateTime.Now;
            }

            publicacion.Activo = true;
            publicacion.CreatedAt = DateTime.Now;

            _repository.Add(publicacion);
            _repository.Save();
        }
    }
}
