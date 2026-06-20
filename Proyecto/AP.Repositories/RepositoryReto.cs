using System.Collections.Generic;
using System.Linq;
using AP.Data.Entities;

namespace AP.Repositories
{
    /// <summary>
    /// Contrato específico de retos: hereda el CRUD genérico y
    /// agrega solo las consultas propias de la entidad.
    /// </summary>
    public interface IRepositoryReto : IRepositoryBase<Reto>
    {
        IEnumerable<Reto> GetActivosPorModo(ModoJuego modo);
    }

    /// <summary>
    /// Repositorio concreto de Reto. Fíjate lo corto que es:
    /// todo el CRUD viene gratis de RepositoryBase&lt;Reto&gt;.
    /// </summary>
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
