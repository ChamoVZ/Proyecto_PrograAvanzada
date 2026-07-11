using System.Collections.Generic;
using System.Linq;
using AP.Data.Entities;

namespace AP.Repositories
{
    public interface IRepositorySolicitudTI : IRepositoryBase<SolicitudTI>
    {
        IEnumerable<SolicitudTI> GetPorUsuario(string usuarioId);
        IEnumerable<SolicitudTI> GetPorEstado(EstadoSolicitud estado);
    }

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
