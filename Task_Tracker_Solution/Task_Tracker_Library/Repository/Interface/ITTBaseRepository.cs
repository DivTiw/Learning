using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_Library.Repository;

namespace Task_Tracker_Library.Interface
{
    public interface ITTBaseRepository<TEntity> where TEntity : class
    {
        TEntity Get(int id);
        bool Any(Expression<Func<TEntity, bool>> condition);
        IEnumerable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] navigationProperties);
        IEnumerable<TEntity> GetList(Func<TEntity, bool> where, params Expression<Func<TEntity, object>>[] navigationProperties);
        IEnumerable<TSource> GetList<TSource>(Expression<Func<TSource, bool>> where, params Expression<Func<TSource, object>>[] navigationProperties) where TSource : class;
        TEntity GetSingle(Func<TEntity, bool> where, params Expression<Func<TEntity, object>>[] navigationProperties);
        IDictionary<string, object> GetEntitiesByName(IList<string> lstEntityNames);//, Func<string, object>[] abc
        object GetEntityByName(string entityName);

        OperationDetailsDTO saveOperation(TEntity EntitObj, EntityState Operation, bool isSoftDelete = true);
        void Add(TEntity items);
        void AddRange(IEnumerable<TEntity> items);

        void Update(TEntity items);
        void UpdateRange(IEnumerable<TEntity> items);

        void Remove(TEntity items);
        void RemoveRange(IEnumerable<TEntity> items);
    }
}
