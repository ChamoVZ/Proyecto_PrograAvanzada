using System.Collections.Generic;
using System.Linq;
using AP.Data.Entities;

namespace AP.Repositories
{
    public interface IRepositoryPublicacion : IRepositoryBase<Publicacion>
    {
        IEnumerable<Publicacion> GetActivasRecientes();
    }

    public class RepositoryPublicacion : RepositoryBase<Publicacion>, IRepositoryPublicacion
    {
        public IEnumerable<Publicacion> GetActivasRecientes()
        {
            return Context.Publicaciones
                .Where(p => p.Activo)
                .OrderByDescending(p => p.FechaPublicacion)
                .Take(20)
                .ToList();
        }
    }
}
