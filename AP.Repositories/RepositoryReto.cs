using System.Collections.Generic;
using System.Linq;
using AP.Data.Entities;

namespace AP.Repositories
{
    /// <summary>
    /// Contrato específico de retos: hereda el CRUD genérico y
    /// agrega solo las consultas propias de la entidad.
    /// </summary>
    // SOLID: ISP - contrato especifico de retos; solo expone las consultas que su cliente necesita.
    public interface IRepositoryReto : IRepositoryBase<Reto>
    {
        IEnumerable<Reto> GetActivosPorModo(ModoJuego modo);
    }

    /// <summary>
    /// Repositorio concreto de Reto. Fíjate lo corto que es:
    /// todo el CRUD viene gratis de RepositoryBase&lt;Reto&gt;.
    /// </summary>
    // SOLID: LSP - hereda el CRUD generico de RepositoryBase sin alterar su comportamiento.
    public class RepositoryReto : RepositoryBase<Reto>, IRepositoryReto
    {
        public IEnumerable<Reto> GetActivosPorModo(ModoJuego modo)
        {
            return Context.Retos
                .Where(r => r.Activo && r.Modo == modo)
                .OrderBy(r => r.Dificultad)
                .ToList();
        }
    }
}
