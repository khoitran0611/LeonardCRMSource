namespace Elinext.BusinessLib
{
    using System;
    using System.Linq.Expressions;
    using System.Collections.Generic;

    public interface IBusinessBase<TEntity> where TEntity:class
    {
        /// <summary>
        /// Insert Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Insert(TEntity entity);
        /// <summary>
        /// Update Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Update(TEntity entity);
        /// <summary>
        /// Delete Entity
        /// </summary>
        /// <param name="entity"></param>
        int Delete(TEntity entity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        int Insert(IList<TEntity> listEntity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        int Update(IList<TEntity> listEntity);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listEntity"></param>
        int Delete(IList<TEntity> listEntity);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<TEntity> GetAll();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity First(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// Return an entity with its additional reference objects, eg. User includes Roles, Permissions...
        /// </summary>
        /// <param name="predicate">A where condition to filer for entity</param>
        /// <param name="referenceProperties">The reference property names of current entity, eg. Roles, Permissions...</param>
        /// <returns></returns>
        TEntity SingleLoadWithReferences(Expression<Func<TEntity, bool>> predicate, params string[] referenceProperties);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IList<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wherePredicate"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderPredicate"></param>
        /// <param name="descending"></param>
        /// <returns></returns>
        IList<TEntity> SelectWithPaging(Expression<Func<TEntity, bool>> wherePredicate, int? pageIndex,
                                              int? pageSize, Func<TEntity, object> orderPredicate, bool? desc);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteSqlCommand(string commandStr, params object[] parameters);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IList<TEntity> ExecuteSqlQuery(string commandStr, params object[] parameters);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteStoreCommand(string commandStr, params object[] parameters);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandStr"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IList<TEntity> ExecuteStoreQuery(string commandStr, params object[] parameters);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int Count();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Count(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetById(params object[] id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Max(Expression<Func<TEntity, int>> predicate);
    }
}
