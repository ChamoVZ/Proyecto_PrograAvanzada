using System;
using System.Collections.Generic;
using AP.Core.Exceptions;
using AP.Data;
using AP.Data.Entities;
using AP.Repositories;

namespace AP.Core.Business
{
    // Controller -> Business -> Repository -> Data. Nunca saltarse capas.
    // SOLID: SRP - esta clase solo contiene las reglas de negocio de los retos.
    public class RetoBusiness
    {
        // SOLID: DIP - depende de la abstraccion IRepositoryReto, no de la implementacion concreta.
        private readonly IRepositoryReto _repositoryReto;

        public RetoBusiness()
        {
            _repositoryReto = new RepositoryReto();
        }

        // El contexto lo abre y lo libera el controller, para que no quede colgado tras el request.
        public RetoBusiness(MathemaXContext context)
        {
            _repositoryReto = new RepositoryReto(context);
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

        public bool SaveOrUpdate(Reto reto)
        {
            // Regla de negocio: vive AQUÍ, no en el controller ni en la vista.
            if (reto.Dificultad < 1 || reto.Dificultad > 5)
                throw new AppException("La dificultad del reto debe estar entre 1 y 5.");

            if (reto.TiempoLimiteSegundos <= 0)
                throw new AppException("El reto debe tener un tiempo límite mayor a cero.");

            // Sin esto se guarda un reto con un modo que ningún controlador consulta, y queda invisible.
            if (!Enum.IsDefined(typeof(ModoJuego), reto.Modo))
                throw new AppException("El modo de juego seleccionado no es válido.");

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

        public void Desactivar(int id)
        {
            var reto = _repositoryReto.GetById(id);
            if (reto == null)
                throw new AppException("El reto que intenta desactivar no existe.");

            // Borrado lógico: el FK de Partidas está en cascada, así que un borrado físico se llevaría
            // el historial jugado y dejaría el marcador desincronizado del perfil.
            reto.Activo = false;
            reto.LastModified = DateTime.Now;
            _repositoryReto.Update(reto);
        }
    }
}
