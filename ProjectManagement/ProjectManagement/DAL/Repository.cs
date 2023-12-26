using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using ProjectManagement.DAL;
using ProjectManagement.DAL.DbModel;

namespace SignalRDemo.DAL
{
    //http://stackoverflow.com/questions/8348175/how-to-add-documentation-tooltip-to-classes-methods-properties-etc-in-c

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly CellPhoneProjectEntities _context;

        public Repository(CellPhoneProjectEntities context)
        {
            _context = context;
        }
        public TEntity Get(long id, string includeProperties = "")
        {
            var dbSet = _context.Set<TEntity>();
            foreach (var includeProperty in includeProperties.Split
                (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                dbSet.Include(includeProperty);
            }
            return dbSet.Find(id);
        }

        public List<TEntity> GetAll(string includeProperties = "")
        {
            var dbSet = _context.Set<TEntity>();
            foreach (var includeProperty in includeProperties.Split
                (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                dbSet.Include(includeProperty);
            }
            return dbSet.ToList();
        }

        public List<TEntity> Find(Expression<Func<TEntity, bool>> predicate, string includeProperties = "")
        {
            var dbSet = _context.Set<TEntity>();
            foreach (var includeProperty in includeProperties.Split
                (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                dbSet.Include(includeProperty);
            }
            return dbSet.Where(predicate).ToList();
        }

        public void Add(TEntity entiry)
        {
            _context.Set<TEntity>().Add(entiry);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
        }

        public void Update(TEntity entityToUpdate)
        {
            _context.Set<TEntity>().Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }
        public void UpdateRange(IEnumerable<TEntity> entitiesToUpdate)
        {
            foreach (var entityToUpdate in entitiesToUpdate)
            {
                _context.Set<TEntity>().Attach(entityToUpdate);
                _context.Entry(entityToUpdate).State = EntityState.Modified;
            }
        }
        public void Remove(object id)
        {
            TEntity entityToDelete = _context.Set<TEntity>().Find(id);
            if (entityToDelete == null)
            {
                throw new Exception("Entry not found");
            }
            Remove(entityToDelete);
        }

        public void Remove(TEntity entiry)
        {
            _context.Set<TEntity>().Remove(entiry);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _context.Set<TEntity>().Remove(entity);
            }
        }
    }
}