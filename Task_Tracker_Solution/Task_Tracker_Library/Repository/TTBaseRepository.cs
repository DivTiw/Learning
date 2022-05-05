using LinqKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Library.Interface;

namespace Task_Tracker_Library.Repository
{
    public abstract class TTBaseRepository<TEntity> : ITTBaseRepository<TEntity> where TEntity : class
    {
        protected TTDBContext context { get; }
        protected IDbSet<TEntity> dbSet;

        public TTBaseRepository(TTDBContext _context)
        {
            context = _context;
            dbSet = context.Set<TEntity>();
        }
        #region Get Functions
        public virtual TEntity Get(int id)
        {
            return context.Set<TEntity>().Find(id);
        }
        public virtual bool Any(Expression<Func<TEntity, bool>> condition)
        {
            return context.Set<TEntity>().Any(condition);
        }
        public virtual IEnumerable<TEntity> GetAll(params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            IEnumerable<TEntity> list;

            IQueryable<TEntity> dbQuery = context.Set<TEntity>();
            
            if (navigationProperties != null)
            {
                //Apply eager loading
                foreach (Expression<Func<TEntity, object>> navigationProperty in navigationProperties)
                    dbQuery = dbQuery.Include<TEntity, object>(navigationProperty);
            }

            list = dbQuery
                .AsNoTracking();

            return list;
        }
        public virtual IEnumerable<TEntity> GetList(Func<TEntity, bool> where, params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            IEnumerable<TEntity> list;
            IQueryable<TEntity> dbQuery = context.Set<TEntity>();

            //Apply eager loading
            foreach (Expression<Func<TEntity, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<TEntity, object>(navigationProperty);

            list = dbQuery
                .AsNoTracking()
                .Where(where);
            return list;
        }
        public virtual IEnumerable<TSource> GetList<TSource>(Expression<Func<TSource, bool>> where,
             params Expression<Func<TSource, object>>[] navigationProperties) where TSource : class
        {
            IEnumerable<TSource> list;
            IQueryable<TSource> dbQuery = context.Set<TSource>();

            //Apply eager loading
            foreach (Expression<Func<TSource, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<TSource, object>(navigationProperty);

            list = dbQuery
                .AsNoTracking()
                .AsExpandable() //from LinqKit
                .Where(where);

            return list;
        }

        /// <summary>
        /// Return any table data having it's records is_deleted false.
        /// </summary>
        /// <param name="lstEntityNames"></param>
        /// <param name="abc"></param>
        /// <returns></returns>
        public virtual IDictionary<string, object> GetEntitiesByName(IList<string> lstEntityNames)//, Func<string, object>[] abc
        {
            if (lstEntityNames.Count <= 0)
                throw new ArgumentNullException("lstEntityNames", "List of entity names can not be null. Please mention at least one entity for fetching the data.");

            IDictionary<string, object> dicTableData = new Dictionary<string, object>();

            foreach (string entityName in lstEntityNames)
            {
                var entityData = GetEntityByName(entityName);
                dicTableData.Add(entityName, entityData);
            }

            return dicTableData;
        }

        public object GetEntityByName(string entityName)
        {
            Type entityType = context.GetType().GetProperty(entityName).PropertyType.GenericTypeArguments[0];

            if (entityType == null)
                throw new MissingMemberException($"Requested entity '{entityName}' is not present in the DB context of the entities.");

            Type entityListType = typeof(List<>).MakeGenericType(entityType);
            IList lstEntityData = (IList)Activator.CreateInstance(entityListType);
            MethodInfo fetchMethod = GetType().GetMethod("fetchEntity");
            MethodInfo genericFetchMethod = fetchMethod.MakeGenericMethod(entityType);
            PropertyInfo propIsDeleted = entityType.GetProperty(nameof(JMBaseBusinessEntity.is_deleted));//"is_deleted"

            lstEntityData = (IList)genericFetchMethod.Invoke(this, new object[] { context, propIsDeleted, false });
            return lstEntityData;
        }
        public virtual TEntity GetSingle(Func<TEntity, bool> where,
            params Expression<Func<TEntity, object>>[] navigationProperties)
        {
            return GetList(where).FirstOrDefault();
            //TEntity item = null;
            ////using (var context = new TTDBContext())
            ////{
            //IQueryable<TEntity> dbQuery = context.Set<TEntity>();

            ////Apply eager loading
            //foreach (Expression<Func<TEntity, object>> navigationProperty in navigationProperties)
            //    dbQuery = dbQuery.Include<TEntity, object>(navigationProperty);

            //item = dbQuery
            //    .AsNoTracking() //Don't track any changes for the selected item
            //    .FirstOrDefault(where); //Apply where clause
            ////}
            //return item;
        }

        /// <summary>
        /// This method is referenced by "GetEntityByName" method in this class using reflection.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="context"></param>
        /// <param name="prop"></param>
        /// <param name="propVal"></param>
        /// <returns></returns>
        public IList<TResult> fetchEntity<TResult>(TTDBContext context, PropertyInfo prop, object propVal) where TResult : class//TResult entityType, , Expression<Func<TResult, bool>> where = null
        {
            //IList <TResult> lst = new List<TResult>();
            IQueryable<TResult> dbQuery = context.Set<TResult>();

            //Apply eager loading
            //foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
            //    dbQuery = dbQuery.Include<T, object>(navigationProperty);

            //PropertyInfo propIsDeleted = typeof(TResult).GetProperty("is_deleted");
            //if (propIsDeleted != null)
            //{
            //    dbQuery = dbQuery.Where(x => !x.Equals(null));//GetType().GetProperty("is_deleted")GetValue(null, null).Equals(0)
            //}

            if (prop != null && propVal != null)
            {
                dbQuery = dbQuery.Where(PredicateBuilder.expressPropertyCompare<TResult>(propVal, prop));
            }

            var lst = dbQuery
                .AsNoTracking()
                .ToList();
            return lst;
        }
        #endregion Get Funtions


        #region Add, Update, Save, and Remove
        public OperationDetailsDTO saveOperation(TEntity EntitObj, EntityState Operation, bool isSoftDelete = true)
        {
            ///ToDo: Before insertion check if record already present and then insert.
            if (EntitObj == null)
            {
                throw new ArgumentNullException("EntitObj", "Parameter 'EntityObj' can not be null");
            }
            OperationDetailsDTO od = new OperationDetailsDTO();

            EntityState op = Operation;
            //using (var context = new TTDBContext())
            //{
            int NoOfRowsUpdated = 0;

            #region Change Operation for Soft Delete
            if (Operation.Equals(EntityState.Deleted) && isSoftDelete)
            {
                Type t = EntitObj.GetType();
                PropertyInfo prop = t.GetProperty("is_deleted", BindingFlags.Public);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(EntitObj, true);
                    op = EntityState.Modified;
                }
            }
            #endregion

            //context.Entry(EntitObj).State = op;
            //NoOfRowsUpdated = context.SaveChanges();

            ///ToDo: fetch database operation messages from DB.
            if (NoOfRowsUpdated > 0)
            {
                od.opStatus = true;
                switch (Operation)
                {
                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Added:
                        Add(EntitObj);
                        od.opMsg = "Record Added successfully";
                        break;
                    case EntityState.Deleted:
                        od.opMsg = "Record deleted successfully";
                        break;
                    case EntityState.Modified:
                        od.opMsg = "Record updated successfully";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                od.opStatus = false;
            }
            //}
            return od;
        }
        public virtual void Add(TEntity item)
        {
            //foreach (TEntity item in items)
            //{
            dbSet.Add(item);
            //context.Entry(item).State = EntityState.Added;
            //}
            //context.SaveChanges();
        }
        public virtual void AddRange(IEnumerable<TEntity> items)
        {
            context.Set<TEntity>().AddRange(items);
            //context.Entry(item).State = EntityState.Added;
            //context.SaveChanges();
        }
        public virtual void Update(TEntity item)
        {
            ///ToDo: Update only those properties which are updated and not all of them.
            //foreach (TEntity item in items)
            //{
            context.Entry(item).State = EntityState.Modified;
            //}
            //context.SaveChanges();
        }
        public virtual void UpdateRange(IEnumerable<TEntity> items)
        {
            foreach (TEntity item in items)
            {
                Update(item);
                //context.Entry(item).State = EntityState.Modified;
            }
            //context.SaveChanges();
        }
        public virtual void Remove(TEntity item)
        {
            throw new NotImplementedException();
            //foreach (TEntity item in items)
            //{
            //    context.Entry(item).State = EntityState.Deleted;
            //}
            //context.SaveChanges();
        }
        public virtual void RemoveRange(IEnumerable<TEntity> items)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
