using System.Collections.Generic;
using System.Linq;
using AP.Data;
using AP.Data.Entities;

namespace AP.Repositories
{
    // SOLID: ISP - contrato especifico de soporte; solo expone las consultas que su cliente necesita.
    public interface IRepositorySolicitudTI : IRepositoryBase<SolicitudTI>
    {
        IEnumerable<SolicitudTI> GetActivas();
        IEnumerable<SolicitudTI> GetPorUsuario(string usuarioId);
    }

    // SOLID: LSP - hereda el CRUD generico de RepositoryBase sin alterar su comportamiento.
    public class RepositorySolicitudTI : RepositoryBase<SolicitudTI>, IRepositorySolicitudTI
    {
        public RepositorySolicitudTI()
        {
        }

        public RepositorySolicitudTI(MathemaXContext context) : base(context)
        {
        }

        public IEnumerable<SolicitudTI> GetActivas()
        {
            return Context.SolicitudesTI
                .Where(s => s.Activo)
                .OrderByDescending(s => s.FechaCreacion)
                .ToList();
        }

        public IEnumerable<SolicitudTI> GetPorUsuario(string usuarioId)
        {
            return Context.SolicitudesTI
                .Where(s => s.UsuarioId == usuarioId && s.Activo)
                .OrderByDescending(s => s.FechaCreacion)
                .ToList();
        }
    }
}
