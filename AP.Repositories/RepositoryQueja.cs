using System.Collections.Generic;
using System.Linq;
using AP.Data;
using AP.Data.Entities;

namespace AP.Repositories
{
    // SOLID: ISP - contrato especifico del buzon; solo expone las consultas que su cliente necesita.
    public interface IRepositoryQueja : IRepositoryBase<Queja>
    {
        IEnumerable<Queja> GetActivas();
        IEnumerable<Queja> GetPorUsuario(string usuarioId);
    }

    // SOLID: LSP - hereda el CRUD generico de RepositoryBase sin alterar su comportamiento.
    public class RepositoryQueja : RepositoryBase<Queja>, IRepositoryQueja
    {
        public RepositoryQueja()
        {
        }

        public RepositoryQueja(MathemaXContext context) : base(context)
        {
        }

        public IEnumerable<Queja> GetActivas()
        {
            return Context.Quejas
                .Where(q => q.Activo)
                .OrderByDescending(q => q.FechaCreacion)
                .ToList();
        }

        public IEnumerable<Queja> GetPorUsuario(string usuarioId)
        {
            return Context.Quejas
                .Where(q => q.UsuarioId == usuarioId && q.Activo)
                .OrderByDescending(q => q.FechaCreacion)
                .ToList();
        }
    }
}
