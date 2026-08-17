using System.Collections.Generic;
using System.Linq;
using AP.Data;
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

    public class HistorialUsuario
    {
        public System.DateTime FechaJuego { get; set; }
        public bool Acertado { get; set; }
        public int TiempoEmpleadoSegundos { get; set; }
        public int XpGanado { get; set; }
        public ModoJuego Modo { get; set; }
        public string TituloReto { get; set; }
    }

    // SOLID: ISP - contrato especifico de partidas; solo expone el historial y el ranking que su cliente necesita.
    public interface IRepositoryPartida : IRepositoryBase<Partida>
    {
        IEnumerable<HistorialUsuario> GetHistorialPorUsuario(
            string usuarioId,
            bool? resultado,
            ModoJuego? modo,
            System.DateTime? desde,
            System.DateTime? hasta,
            int pagina,
            int tamanoPagina);
        int ContarHistorialPorUsuario(
            string usuarioId,
            bool? resultado,
            ModoJuego? modo,
            System.DateTime? desde,
            System.DateTime? hasta);
        IEnumerable<RankingUsuario> GetRankingGlobal(int pagina, int tamanoPagina);
        int ContarRankingGlobal();
    }

    // SOLID: LSP - hereda el CRUD generico de RepositoryBase sin alterar su comportamiento.
    public class RepositoryPartida : RepositoryBase<Partida>, IRepositoryPartida
    {
        public RepositoryPartida(MathemaXContext context) : base(context)
        {
        }

        public IEnumerable<HistorialUsuario> GetHistorialPorUsuario(
            string usuarioId,
            bool? resultado,
            ModoJuego? modo,
            System.DateTime? desde,
            System.DateTime? hasta,
            int pagina,
            int tamanoPagina)
        {
            return FiltrarHistorial(usuarioId, resultado, modo, desde, hasta)
                .OrderByDescending(p => p.FechaJuego)
                .ThenByDescending(p => p.PartidaId)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Select(p => new HistorialUsuario
                {
                    FechaJuego = p.FechaJuego,
                    Acertado = p.Acertado,
                    TiempoEmpleadoSegundos = p.TiempoEmpleadoSegundos,
                    XpGanado = p.XpGanado,
                    Modo = p.Reto.Modo,
                    TituloReto = p.Reto.Titulo
                })
                .ToList();
        }

        public int ContarHistorialPorUsuario(
            string usuarioId,
            bool? resultado,
            ModoJuego? modo,
            System.DateTime? desde,
            System.DateTime? hasta)
        {
            return FiltrarHistorial(usuarioId, resultado, modo, desde, hasta).Count();
        }

        public IEnumerable<RankingUsuario> GetRankingGlobal(int pagina, int tamanoPagina)
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
                .ThenBy(r => r.UsuarioId)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();
        }

        public int ContarRankingGlobal()
        {
            return Context.Partidas.Select(p => p.UsuarioId).Distinct().Count();
        }

        private IQueryable<Partida> FiltrarHistorial(
            string usuarioId,
            bool? resultado,
            ModoJuego? modo,
            System.DateTime? desde,
            System.DateTime? hasta)
        {
            var query = Context.Partidas.Where(p => p.UsuarioId == usuarioId);

            if (resultado.HasValue)
            {
                query = query.Where(p => p.Acertado == resultado.Value);
            }

            if (modo.HasValue)
            {
                query = query.Where(p => p.Reto.Modo == modo.Value);
            }

            if (desde.HasValue)
            {
                var desdeInclusive = desde.Value.Date;
                query = query.Where(p => p.FechaJuego >= desdeInclusive);
            }

            if (hasta.HasValue)
            {
                var hastaExclusiva = hasta.Value.Date.AddDays(1);
                query = query.Where(p => p.FechaJuego < hastaExclusiva);
            }

            return query;
        }
    }
}
