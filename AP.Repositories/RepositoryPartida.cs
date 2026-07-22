using System.Collections.Generic;
using System.Linq;
using AP.Data.Entities;

namespace AP.Repositories
{
    // Fila del ranking global; solo lleva el UsuarioId, el nombre se resuelve en la capa MVC.
    public class RankingUsuario
    {
        public string UsuarioId { get; set; }
        public int ExperienciaTotal { get; set; }
        public int PartidasJugadas { get; set; }
        public int Aciertos { get; set; }
    }

    public interface IRepositoryPartida : IRepositoryBase<Partida>
    {
        IEnumerable<Partida> GetHistorialPorUsuario(string usuarioId);
        IEnumerable<RankingUsuario> GetRankingGlobal();
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

        public IEnumerable<RankingUsuario> GetRankingGlobal()
        {
            return Context.Partidas
                .GroupBy(p => p.UsuarioId)
                .Select(g => new RankingUsuario
                {
                    UsuarioId = g.Key,
                    ExperienciaTotal = g.Sum(p => p.XpGanado),
                    PartidasJugadas = g.Count(),
                    Aciertos = g.Sum(p => p.Acertado ? 1 : 0)
                })
                .OrderByDescending(r => r.ExperienciaTotal)
                .ToList();
        }
    }
}
