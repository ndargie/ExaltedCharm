using System;
using System.Threading.Tasks;
using ExaltedCharm.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExaltedCharm.Api.Services
{
    public class EntityFrameworkRepository<TContext> : EntityFrameworkReadOnlyRepository<TContext>, IRepository
        where TContext : DbContext
    {
        private readonly TContext _context;

        public EntityFrameworkRepository(TContext context) : base(context)
        {
            _context = context;
        }

        public void Create<TEntity>(TEntity entity, string createdBy = null) where TEntity : class, IEntity
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.CreatedBy = createdBy;
            _context.Set<TEntity>().Add(entity);
        }

        public void Update<TEntity>(TEntity entity, string modifiedBy = null) where TEntity : class, IEntity
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = modifiedBy;
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete<TEntity>(object id) where TEntity : class, IEntity
        {
            TEntity entity = _context.Set<TEntity>().Find(id);
            Delete(entity);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            var dbSet = _context.Set<TEntity>();
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public bool Save()
        {
           return  _context.SaveChanges() >= 0;
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}