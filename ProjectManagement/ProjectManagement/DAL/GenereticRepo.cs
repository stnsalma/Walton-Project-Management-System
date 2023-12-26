using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ProjectManagement.DAL.DbModel;

namespace ProjectManagement.DAL
{
    public class GenereticRepo<T> where T : class
    {
        //Create
        public static void Add(CellPhoneProjectEntities dbContext, T entity)
        {
            dbContext.Set<T>().Add(entity);
            dbContext.SaveChanges();
        }
        public static void AddList(CellPhoneProjectEntities dbContext, List<T> entity)
        {
            dbContext.Set<T>().AddRange(entity);
            dbContext.SaveChanges();
        }
        //Create ///Get ID of the table
        public static T Add(CellPhoneProjectEntities dbContext, T entity, long id)
        {
            dbContext.Set<T>().Add(entity);
            dbContext.SaveChanges();
            return entity;
        }
        //Read
        public static T GetById(CellPhoneProjectEntities dbContext, long id)
        {
            var model = dbContext.Set<T>().Find(id);
            // _dbContext.Set<T>().Find(id);
            return model;
        }
        public static T Get(CellPhoneProjectEntities dbContext, string query)
        {

            var myModel = dbContext.Database.SqlQuery<T>(query).FirstOrDefault();

            return myModel;

        }
        public static T Get(CellPhoneProjectEntities dbContext, System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {

            // _dbContext.Set<T>().Find(id);
            return dbContext.Set<T>().FirstOrDefault(predicate);
        }

        public static List<T> GetList(CellPhoneProjectEntities dbContext)
        {
            var allList = dbContext.Set<T>().ToList();

            return allList;
        }
        public static List<T> GetList(CellPhoneProjectEntities dbContext, System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {

            return dbContext.Set<T>().Where(predicate).ToList();
        }
        public static List<T> GetList(CellPhoneProjectEntities dbContext, string query)
        {
            var myList = dbContext.Database.SqlQuery<T>(query).ToList();
            return myList;

        }

        //Update
        public static void Update(CellPhoneProjectEntities dbContext, T entity)
        {
            dbContext.Set<T>().Attach(entity);
            dbContext.Entry(entity).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
        public static void UpdateList(CellPhoneProjectEntities dbContext, List<T> entity)
        {
            foreach (T item in entity)
            {
                dbContext.Entry(item).State = EntityState.Modified;
            }
            dbContext.SaveChanges();
        }

        //Delete
        /*
            public static void Delete(CellPhoneProjectEntities _dbContext, T entity)
            {
                _dbContext.Entry(entity).State = EntityState.Deleted;
                _dbContext.SaveChanges();
            }
            public static void DeleteById(CellPhoneProjectEntities _dbContext, int id)
            {

                T entity = _dbContext.Set<T>().Find(id);
                _dbContext.Entry(entity).State = EntityState.Deleted;
                _dbContext.SaveChanges();

            }

            public static void DeleteList(CellPhoneProjectEntities _dbContext, List<T> entityList)
            {
                foreach (var entity in entityList)
                {
                    _dbContext.Entry(entity).State = EntityState.Deleted;
                }
                _dbContext.SaveChanges();

            }
         
        */
    }
}