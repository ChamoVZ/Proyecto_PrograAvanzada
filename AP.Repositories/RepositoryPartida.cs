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

    // SOLID: ISP - contrato especifico de partidas; solo expone el historial y el ranking que su cliente necesita.
    public interface IRepositoryPartida : IRepositoryBase<Partida>
    {
        IEnumerable<Partida> GetHistorialPorUsuario(string usuarioId);
        IEnumerable<RankingUsuario> GetRankingGlobal();
    }

    // SOLID: LSP - hereda el CRUD generico de RepositoryBase sin alterar su comportamiento.
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
