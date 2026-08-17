using System.Collections.Generic;
using System.Linq;
using AP.Data;
using AP.Data.Entities;

namespace AP.Repositories
{
    // SOLID: ISP - contrato especifico del buzon; solo expone las consultas que su cliente necesita.
    public interface IRepositoryQueja : IRepositoryBase<Queja>
    {
        IEnumerable<Queja> GetActivas(int pagina, int tamanoPagina);
        int ContarActivas();
        IEnumerable<Queja> GetPorUsuario(string usuarioId, int pagina, int tamanoPagina);
        int ContarPorUsuario(string usuarioId);
    }

    // SOLID: LSP - hereda el CRUD generico de RepositoryBase sin alterar su comportamiento.
    public class RepositoryQueja : RepositoryBase<Queja>, IRepositoryQueja
    {
        public RepositoryQueja(MathemaXContext context) : base(context)
        {
        }

        public IEnumerable<Queja> GetActivas(int pagina, int tamanoPagina)
        {
            return Context.Quejas
                .Where(q => q.Activo)
                .OrderByDescending(q => q.FechaCreacion)
                .ThenByDescending(q => q.QuejaId)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();
        }

        public int ContarActivas()
        {
            return Context.Quejas.Count(q => q.Activo);
        }

        public IEnumerable<Queja> GetPorUsuario(string usuarioId, int pagina, int tamanoPagina)
        {
            return Context.Quejas
                .Where(q => q.UsuarioId == usuarioId && q.Activo)
                .OrderByDescending(q => q.FechaCreacion)
                .ThenByDescending(q => q.QuejaId)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();
        }

        public int ContarPorUsuario(string usuarioId)
        {
            return Context.Quejas.Count(q => q.UsuarioId == usuarioId && q.Activo);
        }
    }
}
