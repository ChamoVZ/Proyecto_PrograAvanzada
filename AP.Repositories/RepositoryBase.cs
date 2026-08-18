using System;
using System.Data.Entity;
using AP.Data;

namespace AP.Repositories
{
    /// <summary>Contrato genérico de acceso a datos.</summary>
    public interface IRepositoryBase<T> where T : class
    {
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
    }

    // DP: Repository - centraliza el acceso a datos y aisla a la capa de negocio de EF.
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly MathemaXContext Context;

        public RepositoryBase(MathemaXContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
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

        private void Save()
        {
            Context.SaveChanges();
        }
    }
}
