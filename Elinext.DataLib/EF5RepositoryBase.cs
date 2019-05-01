namespace Elinext.DataLib
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    public abstract class EF5RepositoryBase<TContext, TEntity> : IRepository<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        private static string _connectionStr;

        protected EF5RepositoryBase(string connectionStr)
        {
            _connectionStr = connectionStr;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Insert(TEntity entity)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                dbSet.Add(entity);
                return context.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Update(TEntity entity)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                dbSet.Attach(entity);
                context.Entry(entity).State = EntityState.Modified;
                return context.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Delete(TEntity entity)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                dbSet.Attach(entity);
                dbSet.Remove(entity);
                return context.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        /// <returns></returns>
        public virtual int Insert(IList<TEntity> listEntity)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                foreach (TEntity entity in listEntity)
                {
                    dbSet.Add(entity);
                }
                return context.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        /// <returns></returns>
        public virtual int Update(IList<TEntity> listEntity)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                foreach (TEntity entity in listEntity)
                {
                    dbSet.Attach(entity);
                    context.Entry(entity).State = EntityState.Modified;
                }
                return context.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        /// <returns></returns>
        public virtual int Delete(IList<TEntity> listEntity)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                foreach (TEntity entity in listEntity)
                {
                    dbSet.Attach(entity);
                    dbSet.Remove(entity);
                }
                return context.SaveChanges();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IList<TEntity> GetAll()
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.AsNoTracking().ToList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.AsNoTracking().FirstOrDefault(predicate);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.AsNoTracking().SingleOrDefault(predicate);
            }
        }
        /// <summary>
        /// Return an entity with its additional reference objects, eg. User includes Roles, Permissions...
        /// </summary>
        /// <param name="predicate">A where condition to filer for entity</param>
        /// <param name="referenceProperties">The related entity names of current entity, eg. User with related Roles, Permissions...</param>
        /// <returns></returns>
        public TEntity SingleLoadWithReferences(Expression<Func<TEntity, bool>> predicate, params string[] referenceProperties)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                TEntity entity = dbSet.SingleOrDefault(predicate);
                if (entity != null)
                {
                    var properties = typeof(TEntity).GetProperties();
                    foreach (string s in referenceProperties)
                    {
                        var pi = properties.SingleOrDefault(p => p.Name == s);
                        if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                            context.Entry(entity).Collection(s).Load();
                        else
                            context.Entry(entity).Reference(s).Load();
                    }
                }
                return entity;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IList<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.AsNoTracking().Where(predicate).ToList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wherePredicate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderPredicate"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public virtual IList<TEntity> SelectWithPaging(Expression<Func<TEntity, bool>> wherePredicate, int? pageIndex, int? pageSize, Func<TEntity, object> orderPredicate, bool? desc)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                IEnumerable<TEntity> entities;
                //pageIndex = pageIndex.HasValue ? pageIndex.Value - 1 : 0;
                //pageSize = pageSize.HasValue ? pageSize.Value : 20;
                desc = desc.HasValue && desc.Value;
                if (wherePredicate != null && orderPredicate == null)
                    entities = dbSet.AsNoTracking().Where(wherePredicate);
                else if (wherePredicate == null && orderPredicate != null)
                    entities = dbSet.AsNoTracking().OrderBy(orderPredicate);
                else
                    entities = dbSet.AsNoTracking().Where(wherePredicate).OrderBy(orderPredicate);

                if ((bool)desc)
                    entities = entities.Reverse();

                if (pageIndex.HasValue && pageSize.HasValue)
                    entities = entities.Skip((int)pageIndex * (int)pageSize).Take((int)pageSize);

                return entities.ToList();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual int ExecuteSqlCommand(string commandStr, params object[] parameters)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                return context.Database.ExecuteSqlCommand(commandStr, parameters);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IList<TEntity> ExecuteSqlQuery(string commandStr, params object[] parameters)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                IDbCommand storeQuery = context.Database.Connection.CreateCommand();
                storeQuery.CommandText = commandStr;
                storeQuery.CommandType = CommandType.Text;
                if (parameters.Length > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        storeQuery.Parameters.Add(parameter);
                    }
                }
                context.Database.Connection.Open();
                var entities = Activator.CreateInstance<List<TEntity>>();
                using (var reader = storeQuery.ExecuteReader())
                {
                    var properties = typeof(TEntity).GetProperties();
                    while (reader.Read())
                    {
                        var entity = Activator.CreateInstance<TEntity>();
                        foreach (var prop in properties)
                        {
                            if (!prop.PropertyType.IsValueType && prop.PropertyType != typeof(String)) continue;
                            try
                            {
                                prop.SetValue(entity, reader[prop.Name], null);
                            }
                            catch { }
                        }
                        entities.Add(entity);
                    }
                }
                context.Database.Connection.Close();
                return entities;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual int ExecuteStoreCommand(string commandStr, params object[] parameters)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                IDbCommand storeCommand = context.Database.Connection.CreateCommand();
                storeCommand.CommandText = commandStr;
                storeCommand.CommandType = CommandType.StoredProcedure;
                if (parameters.Length > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        storeCommand.Parameters.Add(parameter);
                    }
                }
                return storeCommand.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IList<TEntity> ExecuteStoreQuery(string commandStr, params object[] parameters)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                IDbCommand storeQuery = context.Database.Connection.CreateCommand();
                storeQuery.CommandText = commandStr;
                storeQuery.CommandType = CommandType.StoredProcedure;
                if (parameters.Length > 0)
                {
                    foreach (var parameter in parameters)
                    {
                        storeQuery.Parameters.Add(parameter);
                    }
                }
                context.Database.Connection.Open();
                var entities = Activator.CreateInstance<List<TEntity>>();
                using (var reader = storeQuery.ExecuteReader())
                {
                    var properties = typeof(TEntity).GetProperties();
                    while (reader.Read())
                    {
                        var entity = Activator.CreateInstance<TEntity>();
                        foreach (var prop in properties)
                        {
                            if (!prop.PropertyType.IsValueType && prop.PropertyType != typeof(String)) continue;
                            try
                            {
                                prop.SetValue(entity, reader[prop.Name], null);
                            }
                            catch { }
                        }
                        entities.Add(entity);
                    }
                }
                context.Database.Connection.Close();
                return entities;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual int Count()
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.Count();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.Count(predicate);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetById(params object[] id)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.Find(id);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Max(Expression<Func<TEntity, int>> predicate)
        {
            using (var context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                var dbSet = context.Set<TEntity>();
                return dbSet.Max(predicate);
            }
        }
    }
}
