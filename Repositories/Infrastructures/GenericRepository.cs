using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Infrastructures
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        protected RealtimeQuizDbContext _context;
        protected DbSet<TEntity> _dbSet;
        protected readonly ILogger _logger;

        public GenericRepository(
            RealtimeQuizDbContext context,
            ILogger logger)
        {
            _context = context;
            _logger = logger;
            _dbSet = _context.Set<TEntity>();
        }
        public virtual TEntity AddEntity(TEntity entity) => _dbSet.Add(entity).Entity;

        public virtual TEntity? GetEntityById(int id)
        {
            var result = _dbSet.Find(id);
            if (result != null)
            {
                return result;
            }

            return null;
        }

        //public void UpdateEntity(TEntity entity) => _context.Update(entity).State = EntityState.Modified;

        public virtual void UpdateEntity(TEntity entity)
        {
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;

            if (entity is Room roomEntity)
            {
                entry.Property("RoomCode").IsModified = false;
            }
        }


        public virtual IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }
        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }
        public virtual TEntity GetEntityByGuid(Guid id)
        {
            return _dbSet.Find(id);
        }
        public virtual void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }



    }
}
