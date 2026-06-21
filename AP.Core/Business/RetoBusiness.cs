using System;
using System.Collections.Generic;
using AP.Core.Exceptions;
using AP.Data.Entities;
using AP.Repositories;

namespace AP.Core.Business
{
    // Controller -> Business -> Repository -> Data. Nunca saltarse capas.
    public class RetoBusiness
    {
        private readonly IRepositoryReto _repositoryReto;

        public RetoBusiness()
        {
            _repositoryReto = new RepositoryReto();
        }

        // Constructor para unit tests: permite inyectar un mock del repositorio
        public RetoBusiness(IRepositoryReto repositoryReto)
        {
            _repositoryReto = repositoryReto;
        }

        public IEnumerable<Reto> GetRetos(int id = 0)
        {
            if (id == 0)
                return _repositoryReto.GetAll();

            return new List<Reto> { _repositoryReto.GetById(id) };
        }

        public IEnumerable<Reto> GetRetosPorModo(ModoJuego modo)
        {
            return _repositoryReto.GetActivosPorModo(modo);
        }

        public bool SaveOrUpdate(Reto reto)
        {
            // Regla de negocio: vive AQUÍ, no en el controller ni en la vista.
            if (reto.Dificultad < 1 || reto.Dificultad > 5)
                throw new AppException("La dificultad del reto debe estar entre 1 y 5.");

            if (reto.TiempoLimiteSegundos <= 0)
                throw new AppException("El reto debe tener un tiempo límite mayor a cero.");

            if (reto.RetoId == 0)
            {
                reto.CreatedAt = DateTime.Now;
                reto.Activo = true;
                _repositoryReto.Add(reto);
            }
            else
            {
                reto.LastModified = DateTime.Now;
                _repositoryReto.Update(reto);
            }

            return true;
        }

        public void Delete(int id)
        {
            _repositoryReto.Delete(id);
        }
    }
}
