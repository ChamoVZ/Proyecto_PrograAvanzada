using System;
using System.Collections.Generic;
using AP.Core.Exceptions;
using AP.Data;
using AP.Data.Entities;
using AP.Repositories;

namespace AP.Core.Business
{
    // SOLID: SRP - esta clase solo contiene las reglas de negocio del buzon de quejas.
    public class QuejaBusiness
    {
        // SOLID: DIP - depende de la abstraccion IRepositoryQueja, no de la implementacion concreta.
        private readonly IRepositoryQueja _repository;

        public QuejaBusiness()
        {
            _repository = new RepositoryQueja();
        }

        // Para compartir el contexto con otro Business dentro de la misma transaccion.
        public QuejaBusiness(MathemaXContext context)
        {
            _repository = new RepositoryQueja(context);
        }

        // Inyeccion manual para pruebas o cambios futuros sin tocar el controller.
        public QuejaBusiness(IRepositoryQueja repository)
        {
            _repository = repository;
        }

        public IEnumerable<Queja> GetTodas()
        {
            return _repository.GetActivas();
        }

        public Queja GetPorId(int id)
        {
            return _repository.GetById(id);
        }

        public IEnumerable<Queja> GetPorUsuario(string usuarioId)
        {
            return _repository.GetPorUsuario(usuarioId);
        }

        public void Save(Queja queja)
        {
            if (queja == null)
                throw new AppException("La queja no puede ser nula.");

            if (string.IsNullOrWhiteSpace(queja.Asunto))
                throw new AppException("El asunto de la queja es obligatorio.");

            if (string.IsNullOrWhiteSpace(queja.Descripcion))
                throw new AppException("La descripción de la queja es obligatoria.");

            if (!Enum.IsDefined(typeof(CategoriaQueja), queja.Categoria))
                throw new AppException("La categoría seleccionada no es válida.");

            if (queja.FechaCreacion == default(DateTime))
            {
                queja.FechaCreacion = DateTime.Now;
            }

            queja.Estado = EstadoQueja.Pendiente;
            queja.Activo = true;
            queja.CreatedAt = DateTime.Now;

            _repository.Add(queja);
        }

        // Quien puede editar y quien puede eliminar se decide aca; el controller y la vista
        // solo consultan el resultado.
        public bool PuedeEditar(Queja queja, string usuarioId)
        {
            // Una vez que soporte la movio de Pendiente, el autor ya no la toca.
            return queja != null
                && queja.UsuarioId == usuarioId
                && queja.Estado == EstadoQueja.Pendiente;
        }

        public bool PuedeEliminar(Queja queja, string usuarioId, bool esAdmin)
        {
            return queja != null && (esAdmin || queja.UsuarioId == usuarioId);
        }

        public void Actualizar(Queja queja, string usuarioId)
        {
            if (queja == null)
                throw new AppException("La queja no puede ser nula.");

            if (!PuedeEditar(queja, usuarioId))
                throw new AppException("Solo el autor puede editar su queja, y solo mientras siga pendiente.");

            if (string.IsNullOrWhiteSpace(queja.Asunto))
                throw new AppException("El asunto de la queja es obligatorio.");

            if (string.IsNullOrWhiteSpace(queja.Descripcion))
                throw new AppException("La descripción de la queja es obligatoria.");

            if (!Enum.IsDefined(typeof(CategoriaQueja), queja.Categoria))
                throw new AppException("La categoría seleccionada no es válida.");

            queja.LastModified = DateTime.Now;
            _repository.Update(queja);
        }

        public void Desactivar(int id, string usuarioId, bool esAdmin)
        {
            var queja = _repository.GetById(id);
            if (queja == null)
                throw new AppException("La queja que intenta eliminar no existe.");

            if (!PuedeEliminar(queja, usuarioId, esAdmin))
                throw new AppException("Solo el autor o un administrador pueden eliminar esta queja.");

            // Borrado logico: el registro se conserva pero deja de listarse en el buzon.
            queja.Activo = false;
            queja.LastModified = DateTime.Now;
            _repository.Update(queja);
        }

        public void CambiarEstado(int id, EstadoQueja nuevoEstado)
        {
            if (!Enum.IsDefined(typeof(EstadoQueja), nuevoEstado))
                throw new AppException("El estado seleccionado no es válido.");

            var queja = _repository.GetById(id);
            if (queja == null)
                throw new AppException("La queja que intenta actualizar no existe.");

            queja.Estado = nuevoEstado;
            queja.LastModified = DateTime.Now;
            _repository.Update(queja);
        }
    }
}
