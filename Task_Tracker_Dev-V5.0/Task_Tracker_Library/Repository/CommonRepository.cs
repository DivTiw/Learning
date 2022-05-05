using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Library.Interface;

namespace Task_Tracker_Library.Repository
{
    //Module master as a type is just used for facilitating inheritance, none of the methods from base would be utilised that are directly casted to the ModuleMaster type.
    //Need to look for the better approach.
    public class CommonRepository : TTBaseRepository<ModuleMaster>, ICommonRepository
    {

        public CommonRepository(TTDBContext _context): base(_context)
        {
        }

        //public virtual IList<T> GetList(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        //{
        //    return base.GetList<T>(where, navigationProperties);
        //}
        //public virtual IList<T> GetList(Expression<Func<T, bool>>[] wheres, Expression<Func<T, object>>[] orderbys,
        //     params Expression<Func<T, object>>[] navigationProperties)
        //{
        //    List<T> list;
        //    //using (var context = new TTDBContext())
        //    //{
        //        IQueryable<T> dbQuery = context.Set<T>();

        //        //Apply eager loading
        //        foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
        //            dbQuery = dbQuery.Include<T, object>(navigationProperty);

        //        foreach (Expression<Func<T, bool>> where in wheres)
        //            dbQuery = dbQuery.Where(where);

        //        foreach (Expression<Func<T, object>> orderby in orderbys)
        //            dbQuery = dbQuery.OrderBy(orderby);

        //        list = dbQuery
        //            .AsNoTracking()
        //            .ToList<T>();
        //    //}
        //    return list;
        //}
       
        
        public virtual DDLDTO fillDDLdata(DDLDTO ddlObj)
        {
            if (ddlObj == null) return ddlObj;//|| !ddlObj.shouldGetData

            //if (!ddlObj.shouldGetData) return ddlObj;

            DBTableNameEnums[] distinctEntityNames = ddlObj.EntityNames.Distinct().ToArray();
            if (ddlObj.EntityNames.Count <= 0)
                throw new ArgumentNullException("lstEntityNames", "List of entity names can not be null. Please mention at least one entity for fetching the data.");

            foreach (var entityName in distinctEntityNames)
            {
                //if GetData is false for the entity, then continue with next iteration and skip the current one.
                if (distinctEntityNames.Length > 1 && ddlObj.Predicate[entityName].Any(x => x.Key.Equals("GetData") && x.Value.Equals(false))) continue;

                var predicates = ddlObj.Predicate[entityName].Where(x=> !x.Key.Equals("GetData")).ToList(); //Skipping the predicate with key as GetData which is not the actual condition for DB tables.

                List<SelectItemDTO> lstSelectItem = null;
                switch (entityName)
                {
                    case DBTableNameEnums.WorkflowMaster:
                        var ddlWFData = GetList<WorkflowMaster>(PredicateBuilder.BuildPredicateFromList<WorkflowMaster>(predicates));
                        lstSelectItem = (from a in ddlWFData
                                         select new SelectItemDTO
                                         {
                                             Text = a.workflow_name,
                                             Value = a.workflow_syscode
                                         }).ToList();
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    case DBTableNameEnums.ModuleMaster:
                        var ddlMData = GetList<ModuleMaster>(PredicateBuilder.BuildPredicateFromList<ModuleMaster>(predicates)).OrderBy(x=> x.module_name);
                        lstSelectItem = (from a in ddlMData
                                         select new SelectItemDTO
                                         {
                                             Text = a.module_name,
                                             Value = a.module_syscode
                                         }).ToList();
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    case DBTableNameEnums.ProjectMaster:
                        var ddlPData = GetList<ProjectMaster>(PredicateBuilder.BuildPredicateFromList<ProjectMaster>(predicates)).OrderBy(x=> x.project_name);
                        lstSelectItem = (from a in ddlPData
                                         select new SelectItemDTO
                                         {
                                             Text = a.project_name,
                                             Value = a.project_syscode
                                         }).ToList();
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    case DBTableNameEnums.vw_employee_master:
                        predicates = predicates.Where(x => !x.Key.Equals("is_deleted") && !x.Key.Equals("is_active")).ToList();
                        var ddlVWEmpData = GetList<TT_vw_employee_master>(PredicateBuilder.BuildPredicateFromList<TT_vw_employee_master>(predicates));
                        lstSelectItem = (from a in ddlVWEmpData
                                         where (a.resigned_on == null || a.last_working_date == null)
                                         select new SelectItemDTO
                                         {
                                             Text = a.employee_name,
                                             Value = a.employee_syscode
                                         }).OrderBy(x => x.Text).ToList(); 
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    case DBTableNameEnums.TaskStatusMaster:
                        predicates = predicates.Where(x => !x.Key.Equals("is_deleted") && !x.Key.Equals("is_active")).ToList();
                        var ddlTaskData = GetList<TaskStatusMaster>(PredicateBuilder.BuildPredicateFromList<TaskStatusMaster>(predicates));
                        lstSelectItem = (from a in ddlTaskData
                                         select new SelectItemDTO
                                         {
                                             Text = a.status_name,
                                             Value = a.status_syscode
                                         }).ToList();
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    case DBTableNameEnums.TaskPriorityMaster:
                        predicates = predicates.Where(x => !x.Key.Equals("is_deleted") && !x.Key.Equals("is_active")).ToList();
                        var ddlPriorityData = GetList<TaskPriorityMaster>(PredicateBuilder.BuildPredicateFromList<TaskPriorityMaster>(predicates));
                        lstSelectItem = (from a in ddlPriorityData
                                         select new SelectItemDTO
                                         {
                                             Text = a.priority_name,
                                             Value = a.priority_syscode
                                         }).ToList();
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    case DBTableNameEnums.CategoryMaster:
                        //predicates = predicates.Where(x => !x.Key.Equals("is_deleted") && !x.Key.Equals("is_active")).ToList();
                        var ddlCatData = GetList<CategoryMaster>(PredicateBuilder.BuildPredicateFromList<CategoryMaster>(predicates)).OrderBy(x=> x.category_name);
                        lstSelectItem = (from a in ddlCatData
                                         select new SelectItemDTO
                                         {
                                             Text = a.category_name,
                                             Value = a.category_syscode
                                         }).ToList();
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    case DBTableNameEnums.vw_department_master:
                        predicates = predicates.Where(x => !x.Key.Equals("is_deleted") && !x.Key.Equals("is_active")).ToList();
                        var ddlVWDeptData = GetList<vw_department_master>(PredicateBuilder.BuildPredicateFromList<vw_department_master>(predicates));
                        lstSelectItem = (from a in ddlVWDeptData
                                         select new SelectItemDTO
                                         {
                                             Text = a.department_name,
                                             Value = a.department_syscode
                                         }).ToList();
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    case DBTableNameEnums.GroupMember:                        
                        var ddlGroupMember = GetList<GroupMember>(PredicateBuilder.BuildPredicateFromList<GroupMember>(predicates));
                        
                        lstSelectItem = (from grpM in ddlGroupMember
                                         join emp in context.vw_employee_master on grpM.employee_syscode equals emp.employee_syscode                                                                                 
                                         select new SelectItemDTO
                                        {
                                            Text = emp.employee_name,
                                            Value = grpM.employee_syscode
                                        }).ToList();
                        if (lstSelectItem != null)
                        {
                            var abc = lstSelectItem.GroupBy(x=> new { x.Text, x.Value });
                            ///ToDo: CommonRepo: Find better approach to find the distinct rows from the record.
                            lstSelectItem = abc.Select(x => x.First()).Select(x => new SelectItemDTO { Text = x.Text, Value = x.Value }).ToList();
                        }
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    case DBTableNameEnums.GroupMaster:
                        //predicates = predicates.Where(x => !x.Key.Equals("is_deleted") && !x.Key.Equals("is_active")).ToList();
                        var ddlGroupData = GetList<GroupMaster>(PredicateBuilder.BuildPredicateFromList<GroupMaster>(predicates));
                        lstSelectItem = (from a in ddlGroupData
                                         select new SelectItemDTO
                                         {
                                             Text = a.group_name,
                                             Value = a.group_syscode
                                         }).ToList();
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    case DBTableNameEnums.GroupMasterByEmp:
                        var empPredicateVal = predicates.Where(x => x.Key.Equals("employee_syscode")).FirstOrDefault().Value;
                        int emp_Syscode = Convert.ToInt32(empPredicateVal);
                        predicates = predicates.Where(x => !x.Key.Equals("employee_syscode")).ToList();
                        var ddlGroup = GetList<GroupMaster>(PredicateBuilder.BuildPredicateFromList<GroupMaster>(predicates)).ToList();
                        lstSelectItem = (from grp in ddlGroup
                                         //join mem in context.GroupMember on grp.group_syscode equals mem.group_syscode
                                         where //mem.employee_syscode == emp_Syscode && mem.is_active && !mem.is_deleted
                                         context.GroupMember.Any(x=> x.group_syscode == grp.group_syscode && x.employee_syscode == emp_Syscode && x.is_active && !x.is_deleted)
                                         select new SelectItemDTO
                                         {
                                             Text = grp.group_name,
                                             Value = grp.group_syscode
                                         }).ToList();
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    case DBTableNameEnums.Tasks:
                        var ddlTskData = GetList<JMTask>(PredicateBuilder.BuildPredicateFromList<JMTask>(predicates)).OrderByDescending(x => x.created_on);
                        lstSelectItem = (from a in ddlTskData
                                         where a.parent_task_syscode == null
                                         select new SelectItemDTO
                                         {
                                             Text = a.task_subject,
                                             Value = a.task_syscode
                                         }).ToList();
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    case DBTableNameEnums.EnvironmentMaster:                       
                        var ddlEnvData = GetList<EnvironmentMaster>(PredicateBuilder.BuildPredicateFromList<EnvironmentMaster>(predicates));
                        lstSelectItem = (from a in ddlEnvData
                                         select new SelectItemDTO
                                         {
                                             Text = a.env_name,
                                             Value = a.env_syscode
                                         }).ToList();
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    case DBTableNameEnums.ProjectParameterMaster:
                        var ddlProjectParameterData = GetList<ProjectParameterMaster>(PredicateBuilder.BuildPredicateFromList<ProjectParameterMaster>(predicates));
                        lstSelectItem = (from a in ddlProjectParameterData
                                         select new SelectItemDTO
                                         {
                                             Text = a.parameter_name,
                                             Value = a.parameter_syscode
                                         }).ToList();
                        ddlObj.Data.Add(entityName, lstSelectItem);
                        break;
                    default:
                        ddlObj.opStatus = false;
                        throw new Exception($"Entity name: {entityName} is not yet added in common repository for fetching the DDL data.");
                        //break;
                }
            }
            ddlObj.opStatus = true;
            return ddlObj;
        }
          

        public string getEmployeeNames(IEnumerable<int> arrEmployeeID)
        {
            string emp_names = "";

            arrEmployeeID = arrEmployeeID.Distinct();
            IEnumerable<string> arrEmp = (from vw in context.vw_employee_master
                                          where arrEmployeeID.Contains(vw.employee_syscode ?? 0)
                                          select vw.employee_name
                                            );

            if (arrEmp.Count() > 0)
            {
                emp_names = string.Join(", ", arrEmp);
            }

            return emp_names;
        }
        #region UnusedCode kept for later utilization

        //private IEnumerable<T> GetList<TEntity>(string connectionString, Func<object, T> caster)
        //{
        //    using (var ctx = new DbContext(connectionString))
        //    {
        //        var setMethod = ctx.GetType().GetMethod("Set").MakeGenericMethod(typeof(T));

        //        var querable = ((DbSet<object>)setMethod
        //        .Invoke(this, null))
        //        .AsNoTracking()
        //        .AsQueryable();

        //        return querable
        //            .Select(x => caster(x))
        //            .ToList();
        //    }
        //}

        //string entityName = Name.ToString();
        ////var entityData = GetEntityByName(entityName);
        ////var n = entityData;
        //Type entityType = context.GetType().GetProperty(entityName).PropertyType.GenericTypeArguments[0];

        //if (entityType == null)
        //    throw new MissingMemberException($"Requested entity '{entityName}' is not present in the DB context of the entities.");

        //Type entityListType = typeof(List<>).MakeGenericType(entityType);
        //IList lstEntityData = (IList)Activator.CreateInstance(entityListType);
        //MethodInfo fetchMethod = GetType().GetMethod("fetchEntity");
        //MethodInfo genericFetchMethod = fetchMethod.MakeGenericMethod(entityType);
        //PropertyInfo propIsDeleted = entityType.GetProperty("is_deleted");

        //lstEntityData = (IList)genericFetchMethod.Invoke(this, new object[] { context, propIsDeleted, false });

        ////var lstSelectItemDTO = from a in lstEntityData.AsQueryable()
        ////                       select new SelectItemDTO
        ////                       {

        ////                       };

        //static void WritePK<T>(T item) where T : new()
        //{
        //    // Just grabbing this to get hold of the type name:
        //    var type = item.GetType();

        //    // Get the PropertyInfo object:
        //    var properties = type.GetProperties();
        //    Console.WriteLine("Finding PK for {0}", type.Name);
        //    foreach (var property in properties)
        //    {
        //        var attributes = property.GetCustomAttributes(false);
        //        foreach (var attribute in attributes)
        //        {
        //            if (attribute.GetType() == typeof(PrimaryKeyAttribute))
        //            {
        //                string msg = "The Primary Key for the {0} class is the {1} property";
        //                Console.WriteLine(msg, type.Name, property.Name);
        //            }
        //        }
        //    }
        //}

        //            static void WritePK< T > (T item) where T : new()
        //{
        //    var type = item.GetType();
        //            var properties = type.GetProperties();
        //            Console.WriteLine("Finding PK for {0}", type.Name);
        //            // This replaces all the iteration above:
        //            var property = properties
        //                .FirstOrDefault(p => p.GetCustomAttributes(false)
        //                    .Any(a => a.GetType() == typeof(PrimaryKeyAttribute)));
        //            if (property != null)
        //            {
        //                string msg = "The Primary Key for the {0} class is the {1} property";
        //                Console.WriteLine(msg, type.Name, property.Name);
        //            }
        //        } 
        #endregion

    }
}
