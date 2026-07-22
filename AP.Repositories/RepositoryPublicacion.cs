using System.Collections.Generic;
using System.Linq;
using AP.Data.Entities;

namespace AP.Repositories
{
    // SOLID: ISP - contrato especifico del foro; solo expone las consultas que su cliente necesita.
    public interface IRepositoryPublicacion : IRepositoryBase<Publicacion>
    {
        IEnumerable<Publicacion> GetActivasRecientes();
    }

    // SOLID: LSP - hereda el CRUD generico de RepositoryBase sin alterar su comportamiento.
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
