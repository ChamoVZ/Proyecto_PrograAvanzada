using System;
using System.Collections.Generic;
using AP.Core.Exceptions;
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

        // Inyeccion manual para pruebas o cambios futuros sin tocar el controller.
        public QuejaBusiness(IRepositoryQueja repository)
        {
            _repository = repository;
        }

        public IEnumerable<Queja> GetTodas()
        {
            return _repository.GetAll();
        }

        public IEnumerable<Queja> GetPorUsuario(string usuarioId)
        {
            return _repository.GetPorUsuario(usuarioId);
        }

        public IEnumerable<Queja> GetPorEstado(EstadoQueja estado)
        {
            return _repository.GetPorEstado(estado);
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
            queja.CreatedAt = DateTime.Now;

            _repository.Add(queja);
            _repository.Save();
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
