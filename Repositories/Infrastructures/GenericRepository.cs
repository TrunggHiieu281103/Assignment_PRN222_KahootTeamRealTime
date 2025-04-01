using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void UpdateEntity(TEntity entity) => _context.Update(entity).State = EntityState.Modified;


    }
}
