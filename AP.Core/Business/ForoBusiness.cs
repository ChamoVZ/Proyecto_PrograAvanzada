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

        public ResultadoPaginado<Publicacion> GetActivasRecientes(int pagina, int tamanoPagina)
        {
            var total = _repository.ContarActivas();
            return ResultadoPaginado<Publicacion>.Crear(
                pagina,
                tamanoPagina,
                total,
                _repository.GetActivasRecientes);
        }

        public Publicacion GetPorId(int id)
        {
            return _repository.GetById(id);
        }

        // Quien puede editar o borrar se decide aca; el controller y la vista solo consultan
        // el resultado. El autor gestiona lo suyo y el Admin modera cualquier publicación.
        public bool PuedeModificar(Publicacion publicacion, string usuarioId, bool esAdmin)
        {
            return publicacion != null && (esAdmin || publicacion.UsuarioId == usuarioId);
        }

        public void Actualizar(Publicacion publicacion, string usuarioId, bool esAdmin)
        {
            if (publicacion == null)
                throw new AppException("La publicación no puede ser nula.");

            if (!PuedeModificar(publicacion, usuarioId, esAdmin))
                throw new AppException("Solo el autor o un administrador pueden modificar esta publicación.");

            if (string.IsNullOrWhiteSpace(publicacion.Titulo))
                throw new AppException("El título de la publicación es obligatorio.");

            if (string.IsNullOrWhiteSpace(publicacion.Contenido))
                throw new AppException("El contenido de la publicación no puede estar vacío.");

            publicacion.LastModified = DateTime.Now;
            _repository.Update(publicacion);
        }

        public void Desactivar(int id, string usuarioId, bool esAdmin)
        {
            var publicacion = _repository.GetById(id);
            if (publicacion == null)
                throw new AppException("La publicación que intenta eliminar no existe.");

            if (!PuedeModificar(publicacion, usuarioId, esAdmin))
                throw new AppException("Solo el autor o un administrador pueden eliminar esta publicación.");

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
