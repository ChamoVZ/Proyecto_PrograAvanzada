using System.Collections.Generic;
using System.Linq;
using AP.Data;
using AP.Data.Entities;

namespace AP.Repositories
{
    // SOLID: ISP - contrato especifico de retos; solo expone las consultas que su cliente necesita.
    public interface IRepositoryReto : IRepositoryBase<Reto>
    {
        IEnumerable<Reto> GetActivosPorModo(ModoJuego modo);
        IEnumerable<Reto> GetPagina(int pagina, int tamanoPagina);
        int Contar();
    }

    // SOLID: LSP - hereda el CRUD generico de RepositoryBase sin alterar su comportamiento.
    public class RepositoryReto : RepositoryBase<Reto>, IRepositoryReto
    {
        public RepositoryReto(MathemaXContext context) : base(context)
        {
        }

        public IEnumerable<Reto> GetActivosPorModo(ModoJuego modo)
        {
            return Context.Retos
                .Where(r => r.Activo && r.Modo == modo)
                .OrderBy(r => r.Dificultad)
                .ToList();
        }

        public IEnumerable<Reto> GetPagina(int pagina, int tamanoPagina)
        {
            return Context.Retos
                .OrderBy(r => r.RetoId)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();
        }

        public int Contar()
        {
            return Context.Retos.Count();
        }
    }
}
