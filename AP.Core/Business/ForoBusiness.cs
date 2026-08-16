using System;
using System.Collections.Generic;
using AP.Data;
using AP.Data.Entities;
using AP.Repositories;
using AP.Core.Exceptions;

namespace AP.Core.Business
{
    // SOLID: SRP - esta clase solo contiene las reglas de negocio del foro.
    public class ForoBusiness
    {
        // SOLID: DIP - depende de la abstraccion IRepositoryPublicacion, no de la implementacion concreta.
        private readonly IRepositoryPublicacion _repository;

        public ForoBusiness()
        {
            _repository = new RepositoryPublicacion();
        }

        // Para compartir el contexto con otro Business dentro de la misma transaccion.
        public ForoBusiness(MathemaXContext context)
        {
            _repository = new RepositoryPublicacion(context);
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

        public Publicacion GetPorId(int id)
        {
            return _repository.GetById(id);
        }

        public void Actualizar(Publicacion publicacion)
        {
            if (publicacion == null)
                throw new AppException("La publicación no puede ser nula.");

            if (string.IsNullOrWhiteSpace(publicacion.Titulo))
                throw new AppException("El título de la publicación es obligatorio.");

            if (string.IsNullOrWhiteSpace(publicacion.Contenido))
                throw new AppException("El contenido de la publicación no puede estar vacío.");

            publicacion.LastModified = DateTime.Now;
            _repository.Update(publicacion);
        }

        public void Desactivar(int id)
        {
            var publicacion = _repository.GetById(id);
            if (publicacion == null)
                throw new AppException("La publicación que intenta eliminar no existe.");

            // Borrado lógico: el registro se conserva pero deja de listarse en el foro.
            publicacion.Activo = false;
            publicacion.LastModified = DateTime.Now;
            _repository.Update(publicacion);
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
        }
    }
}
