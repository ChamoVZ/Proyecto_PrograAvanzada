using System.Collections.Generic;
using System.Linq;
using AP.Data;
using AP.Data.Entities;

namespace AP.Repositories
{
    // SOLID: ISP - contrato especifico del foro; solo expone las consultas que su cliente necesita.
    public interface IRepositoryPublicacion : IRepositoryBase<Publicacion>
    {
        IEnumerable<Publicacion> GetActivasRecientes(int pagina, int tamanoPagina);
        int ContarActivas();
    }

    // SOLID: LSP - hereda el CRUD generico de RepositoryBase sin alterar su comportamiento.
    public class RepositoryPublicacion : RepositoryBase<Publicacion>, IRepositoryPublicacion
    {
        public RepositoryPublicacion(MathemaXContext context) : base(context)
        {
        }

        public IEnumerable<Publicacion> GetActivasRecientes(int pagina, int tamanoPagina)
        {
            return Context.Publicaciones
                .Where(p => p.Activo)
                .OrderByDescending(p => p.FechaPublicacion)
                .ThenByDescending(p => p.PublicacionId)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();
        }

        public int ContarActivas()
        {
            return Context.Publicaciones.Count(p => p.Activo);
        }
    }
}
