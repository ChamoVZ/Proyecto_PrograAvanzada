using System.Collections.Generic;
using System.Linq;
using AP.Data.Entities;

namespace AP.Repositories
{
    // SOLID: ISP - contrato especifico de soporte; solo expone las consultas que su cliente necesita.
    public interface IRepositorySolicitudTI : IRepositoryBase<SolicitudTI>
    {
        IEnumerable<SolicitudTI> GetPorUsuario(string usuarioId);
        IEnumerable<SolicitudTI> GetPorEstado(EstadoSolicitud estado);
    }

    // SOLID: LSP - hereda el CRUD generico de RepositoryBase sin alterar su comportamiento.
    public class RepositorySolicitudTI : RepositoryBase<SolicitudTI>, IRepositorySolicitudTI
    {
        public IEnumerable<SolicitudTI> GetPorUsuario(string usuarioId)
        {
            return Context.SolicitudesTI
                .Where(s => s.UsuarioId == usuarioId)
                .OrderByDescending(s => s.FechaCreacion)
                .ToList();
        }

        public IEnumerable<SolicitudTI> GetPorEstado(EstadoSolicitud estado)
        {
            return Context.SolicitudesTI
                .Where(s => s.Estado == estado)
                .OrderByDescending(s => s.FechaCreacion)
                .ToList();
        }
    }
}
