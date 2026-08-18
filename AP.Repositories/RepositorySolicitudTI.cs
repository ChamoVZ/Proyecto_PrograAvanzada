using System.Collections.Generic;
using System.Linq;
using AP.Data;
using AP.Data.Entities;

namespace AP.Repositories
{
    // SOLID: ISP - contrato especifico de soporte; solo expone las consultas que su cliente necesita.
    public interface IRepositorySolicitudTI : IRepositoryBase<SolicitudTI>
    {
        IEnumerable<SolicitudTI> GetActivas(int pagina, int tamanoPagina);
        int ContarActivas();
        IEnumerable<SolicitudTI> GetPorUsuario(string usuarioId, int pagina, int tamanoPagina);
        int ContarPorUsuario(string usuarioId);
    }

    // SOLID: LSP - hereda el CRUD generico de RepositoryBase sin alterar su comportamiento.
    public class RepositorySolicitudTI : RepositoryBase<SolicitudTI>, IRepositorySolicitudTI
    {
        public RepositorySolicitudTI(MathemaXContext context) : base(context)
        {
        }

        public IEnumerable<SolicitudTI> GetActivas(int pagina, int tamanoPagina)
        {
            return Context.SolicitudesTI
                .Where(s => s.Activo)
                .OrderByDescending(s => s.FechaCreacion)
                .ThenByDescending(s => s.SolicitudTIId)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();
        }

        public int ContarActivas()
        {
            return Context.SolicitudesTI.Count(s => s.Activo);
        }

        public IEnumerable<SolicitudTI> GetPorUsuario(string usuarioId, int pagina, int tamanoPagina)
        {
            return Context.SolicitudesTI
                .Where(s => s.UsuarioId == usuarioId && s.Activo)
                .OrderByDescending(s => s.FechaCreacion)
                .ThenByDescending(s => s.SolicitudTIId)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();
        }

        public int ContarPorUsuario(string usuarioId)
        {
            return Context.SolicitudesTI.Count(s => s.UsuarioId == usuarioId && s.Activo);
        }
    }
}
