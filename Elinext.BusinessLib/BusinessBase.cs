namespace Elinext.BusinessLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using DataLib;

    public abstract class BusinessBase<TRepositoryDA, TEntity> : IBusinessBase<TEntity>
        where TEntity : class
        where TRepositoryDA : IRepository<TEntity>
    {
        private readonly TRepositoryDA _repository;
        public BusinessBase(TRepositoryDA repository)
        {
            _repository = repository;
        }
        public virtual int Insert(TEntity entity)
        {
            return _repository.Insert(entity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Update(TEntity entity)
        {
            return _repository.Update(entity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Delete(TEntity entity)
        {
            return _repository.Delete(entity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        /// <returns></returns>
        public virtual int Insert(IList<TEntity> listEntity)
        {
            return _repository.Insert(listEntity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        /// <returns></returns>
        public virtual int Update(IList<TEntity> listEntity)
        {
            return _repository.Update(listEntity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        /// <returns></returns>
        public virtual int Delete(IList<TEntity> listEntity)
        {
            return _repository.Delete(listEntity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IList<TEntity> GetAll()
        {
            return _repository.GetAll();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            return _repository.First(predicate);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return _repository.Single(predicate);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IList<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _repository.Find(predicate);
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
            return _repository.SelectWithPaging(wherePredicate, pageIndex, pageSize, orderPredicate,desc);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual int ExecuteSqlCommand(string commandStr, params object[] parameters)
        {
            return _repository.ExecuteSqlCommand(commandStr, parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IList<TEntity> ExecuteSqlQuery(string commandStr, params object[] parameters)
        {
            return _repository.ExecuteSqlQuery(commandStr, parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual int ExecuteStoreCommand(string commandStr, params object[] parameters)
        {
            return _repository.ExecuteStoreCommand(commandStr, parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IList<TEntity> ExecuteStoreQuery(string commandStr, params object[] parameters)
        {
            return _repository.ExecuteStoreQuery(commandStr, parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual int Count()
        {
            return _repository.Count();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return _repository.Count(predicate);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetById(params object[] id)
        {
            return _repository.GetById(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Max(Expression<Func<TEntity, int>> predicate)
        {
            return _repository.Max(predicate);
        }

        public TEntity SingleLoadWithReferences(Expression<Func<TEntity, bool>> predicate, params string[] referenceProperties)
        {
            return _repository.SingleLoadWithReferences(predicate, referenceProperties);
        }
    }
}
