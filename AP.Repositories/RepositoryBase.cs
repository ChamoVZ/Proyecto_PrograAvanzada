using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AP.Data;

namespace AP.Repositories
{
    /// <summary>Contrato genérico de acceso a datos.</summary>
    public interface IRepositoryBase<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
        void Save();
    }

    // DP: Repository - centraliza el acceso a datos y aisla a la capa de negocio de EF.
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly MathemaXContext Context;

        public RepositoryBase()
            : this(new MathemaXContext())
        {
        }

        public RepositoryBase(MathemaXContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
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
