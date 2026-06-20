using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AP.Data;

namespace AP.Repositories
{
    /// <summary>
    /// Contrato genérico de acceso a datos. Mismo patrón del proyecto de
    /// referencia: el CRUD se define UNA vez y sirve para todas las entidades.
    /// </summary>
    /// <typeparam name="T">Entidad de AP.Data que administra este repositorio.</typeparam>
    public interface IRepositoryBase<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
        void Save();
    }

    /// <summary>
    /// Implementación genérica del repositorio sobre MathemaXContext.
    /// Los repositorios concretos (RepositoryReto, RepositoryPartida...)
    /// heredan de aquí y solo agregan consultas específicas.
    /// </summary>
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        /// <summary>Contexto compartido con las clases hijas para sus consultas LINQ.</summary>
        protected readonly MathemaXContext Context;

        public RepositoryBase()
        {
            Context = new MathemaXContext();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return Context.Set<T>().ToList();
        }

        public virtual T GetById(int id)
        {
            return Context.Set<T>().Find(id);
        }

        public virtual void Add(T entity)
        {
            Context.Set<T>().Add(entity);
            Save();
        }

        public virtual void Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Save();
        }

        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            if (entity == null) return;
            Context.Set<T>().Remove(entity);
            Save();
        }

        public void Save()
        {
            Context.SaveChanges();
        }
    }
}
