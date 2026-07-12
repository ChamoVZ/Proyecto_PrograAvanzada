using System.Collections.Generic;
using System.Linq;
using AP.Data.Entities;

namespace AP.Repositories
{
    public interface IRepositoryPartida : IRepositoryBase<Partida>
    {
        IEnumerable<Partida> GetHistorialPorUsuario(string usuarioId);
    }

    public class RepositoryPartida : RepositoryBase<Partida>, IRepositoryPartida
    {
        public IEnumerable<Partida> GetHistorialPorUsuario(string usuarioId)
        {
            return Context.Partidas
                .Where(p=>p.UsuarioId == usuarioId)
                .OrderByDescending(p=>p.FechaJuego)
                .ToList();
        }
    }
}
