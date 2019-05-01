using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;

namespace Elinext.DataLib
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// A generic repository for working with data in the database
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EFRepositoryBase<TContext, TEntity> : IRepository<TEntity>
        where TEntity : class
        where TContext : ObjectContext
    {
        private TContext _context;
        private IObjectSet<TEntity> _objectSet;

        private static string _connectionStr;

        protected EFRepositoryBase(string connectionStr)
        {
            _connectionStr = connectionStr;
        }

        #region IRepository<T> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Insert(TEntity entity)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                _objectSet = _context.CreateObjectSet<TEntity>();
                _objectSet.AddObject(entity);
                return _context.SaveChanges();

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                _objectSet.Attach(entity);
                _context.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
                return _context.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public virtual int Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                _objectSet.Attach(entity);
                _objectSet.DeleteObject(entity);
                return _context.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        public virtual int Insert(IList<TEntity> listEntity)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                foreach (TEntity item in listEntity)
                {
                    _objectSet.AddObject(item);
                }
                return _context.SaveChanges();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        public virtual int Update(IList<TEntity> listEntity)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                foreach (TEntity item in listEntity)
                {
                    _objectSet.Attach(item);
                    _context.ObjectStateManager.ChangeObjectState(item, EntityState.Modified);
                }
                return _context.SaveChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        public virtual int Delete(IList<TEntity> listEntity)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                foreach (TEntity item in listEntity)
                {
                    _objectSet.Attach(item);
                    _context.CreateObjectSet<TEntity>().DeleteObject(item);
                }
                return _context.SaveChanges();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<TEntity> GetAll()
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                ((ObjectQuery<TEntity>)_objectSet).MergeOption = MergeOption.NoTracking;
                var results = _objectSet.ToList();
                return results;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                ((ObjectQuery<TEntity>)_objectSet).MergeOption = MergeOption.NoTracking;
                return _objectSet.FirstOrDefault(predicate);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                ((ObjectQuery<TEntity>)_objectSet).MergeOption = MergeOption.NoTracking;
                return _objectSet.SingleOrDefault(predicate);
            }
        }
        /// <summary>
        /// Return an entity with its additional reference objects, eg. User includes Roles, Permissions...
        /// </summary>
        /// <param name="predicate">A where condition to filer for entity</param>
        /// <param name="referenceProperties">The reference property names of current entity, eg. Roles, Permissions...</param>
        /// <returns></returns>
        public TEntity SingleLoadWithReferences(Expression<Func<TEntity, bool>> predicate, string[] referenceProperties)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                ((ObjectQuery<TEntity>)_objectSet).MergeOption = MergeOption.NoTracking;
                TEntity entity = _objectSet.SingleOrDefault(predicate);
                if (entity != null)
                {
                    foreach (string s in referenceProperties)
                        _context.LoadProperty(entity, s);
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
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                ((ObjectQuery<TEntity>)_objectSet).MergeOption = MergeOption.NoTracking;
                var results = _objectSet.Where(predicate);
                return results.ToList();
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
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                ((ObjectQuery<TEntity>)_objectSet).MergeOption = MergeOption.NoTracking;
                IEnumerable<TEntity> entities;
                pageIndex = pageIndex.HasValue ? pageIndex.Value - 1 : 0;
                pageSize = pageSize.HasValue ? pageSize.Value : 20;
                desc = desc.HasValue && desc.Value;
                if (wherePredicate != null && orderPredicate == null)
                    entities = _objectSet.Where<TEntity>(wherePredicate);
                else if (wherePredicate == null && orderPredicate != null)
                    entities = _objectSet.OrderBy(orderPredicate);
                else
                    entities = _objectSet.Where<TEntity>(wherePredicate).OrderBy(orderPredicate);

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
        public int ExecuteSqlCommand(string commandStr, params object[] parameters)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                return _context.ExecuteStoreCommand(commandStr, parameters);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IList<TEntity> ExecuteSqlQuery(string commandStr, params object[] parameters)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                ((ObjectQuery<TEntity>)_objectSet).MergeOption = MergeOption.NoTracking;
                var results = _context.ExecuteStoreQuery<TEntity>(commandStr, parameters);
                return results.ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual int ExecuteStoreCommand(string sqlCommand, params object[] parameters)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                DbConnection dbConnection = _context.Connection;
                using (DbCommand sqlcom = dbConnection.CreateCommand())
                {
                    sqlcom.CommandText = sqlCommand;
                    sqlcom.CommandType = CommandType.StoredProcedure;
                    if (parameters.Length > 0)
                    {
                        foreach (var parameter in parameters)
                        {
                            sqlcom.Parameters.Add(parameter);
                        }
                    }
                    return sqlcom.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IList<TEntity> ExecuteStoreQuery(string sqlCommand, params object[] parameters)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                DbConnection dbConnection = _context.Connection;
                using (DbCommand sqlcom = dbConnection.CreateCommand())
                {
                    sqlcom.CommandText = sqlCommand;
                    sqlcom.CommandType = CommandType.StoredProcedure;
                    if (parameters.Length > 0)
                    {
                        foreach (var parameter in parameters)
                        {
                            sqlcom.Parameters.Add(parameter);
                        }
                    }
                    using (DbDataReader dbDataReader = sqlcom.ExecuteReader())
                    {
                        ObjectResult<TEntity> objectResult = _context.Translate<TEntity>(dbDataReader);
                        return objectResult.ToList();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual int Count()
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                return _objectSet.Count();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                return _objectSet.Count(predicate);
            }
        }

        public virtual TEntity GetById(params object[] id)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                var entitySet = _context.CreateObjectSet<TEntity>().EntitySet;
                var primaryKey = entitySet.ElementType.KeyMembers[0];
                EntityKey entityKey = new EntityKey(entitySet.EntityContainer.Name + "." + entitySet.Name, primaryKey.Name, id);
                return (TEntity)_context.GetObjectByKey(entityKey);
            }
        }

        public virtual int Max(Expression<Func<TEntity, int>> predicate)
        {
            using (_context = (TContext)Activator.CreateInstance(typeof(TContext), _connectionStr))
            {
                _objectSet = _context.CreateObjectSet<TEntity>();
                return _objectSet.Max(predicate);
            }
        }
        #endregion
    }
}
