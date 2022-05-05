using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using System.Configuration;

namespace Task_Tracker_Library.Repository
{
    public class TaskRepository : TTBaseRepository<JMTask>
    {
        public TaskRepository(TTDBContext context) : base(context)
        {
        }

        #region List Functions
        public List<TaskDM> getMyTaskList(int userId, SearchDTO searchCriteria = null, bool isCreatedByMe = false, bool isOwnedByMe = false)
        {
            var query = (from task in context.Tasks
                         join sts in context.TaskStatusMaster on task.task_status_syscode equals sts.status_syscode
                         join mod in context.ModuleMaster on new { col1 = task.module_syscode ?? 0, col2 = false, col3 = true } equals new { col1 = mod.module_syscode, col2 = mod.is_deleted, col3 = mod.is_active } into modTaskGrp
                         from modTask in modTaskGrp.DefaultIfEmpty()
                         join proj in context.ProjectMaster on new { col1 = modTask.project_syscode, col2 = false, col3 = true } equals new { col1 = proj.project_syscode, col2 = proj.is_deleted, col3 = proj.is_active } into projModGrp
                         from prjMod in projModGrp.DefaultIfEmpty()
                         join vwEmp in context.vw_employee_master on task.task_owner equals vwEmp.employee_syscode into empGrp
                         from g in empGrp.DefaultIfEmpty()
                         join vwEmp1 in context.vw_employee_master on task.modified_by equals vwEmp1.employee_syscode into empGrp1
                         from g1 in empGrp1.DefaultIfEmpty()
                         join p in context.Tasks on task.parent_task_syscode equals p.task_syscode into parentGrp
                         from parent in parentGrp.DefaultIfEmpty()
                         join prog in context.ProgressMaster on new { col1 = task.task_syscode, col2 = Enum_Master.ProgressTypeEnum.Task.ToString() } equals new { col1 = prog.type_syscode, col2 = prog.type_detail } into progTaskGrp
                         from progTask in progTaskGrp.DefaultIfEmpty()
                         join pri in context.TaskPriorityMaster on task.task_priority_syscode equals pri.priority_syscode into taskPriGrp
                         from taskPri in taskPriGrp.DefaultIfEmpty()

                         select new TaskDM
                         {
                             task_syscode = task.task_syscode,
                             task_reference = task.task_reference,
                             status = sts,     
                             group_syscode = prjMod.group_syscode,                        
                             project = prjMod,                             
                             module = modTask,
                             task_subject = task.task_subject,
                             owner = g.employee_name,
                             target_date = task.target_date,
                             parent_task_syscode = task.parent_task_syscode,
                             parentTaskRef = parent.task_reference,
                             parentTaskSubject = parent.task_subject,
                             progress = progTask.progress,
                             is_deleted = task.is_deleted,
                             is_active = task.is_active,
                             created_by = task.created_by,
                             created_on = task.created_on,
                             task_status_syscode = task.task_status_syscode,
                             weightage = task.weightage,
                             module_syscode = task.module_syscode,
                             level_syscode = task.level_syscode,
                             priority = taskPri,
                             task_owner = task.task_owner,
                             modified_by_Name = g1.employee_name,
                             modified_on = task.modified_on,
                             isTaskStarted = context.TaskUserRecord.Where(x => x.task_syscode == task.task_syscode && x.employee_syscode == userId && x.stop_time == null).Max(x => x.start_time) == null ? false : true,
                             lstTrail = context.TaskTrail.Where(t => t.task_syscode == task.task_syscode && t.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments).OrderByDescending(x => x.created_on).Take(1).ToList()
                         });
            if (searchCriteria != null)
            {
                if (searchCriteria.task_syscode > 0)
                {
                    query = query.Where(task => task.task_syscode == searchCriteria.task_syscode || task.parent_task_syscode == searchCriteria.task_syscode);
                }
                else if (searchCriteria.module_syscode > 0)
                {
                    query = query.Where(task => task.module_syscode == searchCriteria.module_syscode);
                }
                else
                {
                    if (searchCriteria.project_syscode > 0)
                    {
                        query = query.Where(task => task.project.project_syscode == searchCriteria.project_syscode);
                    }
                    if (searchCriteria.category_syscode > 0)
                    {
                        query = query.Where(task => context.ModuleMaster.Where(x => x.category_syscode == searchCriteria.category_syscode).Any(y => y.module_syscode == task.module_syscode));
                    }
                }
                if (searchCriteria.status_syscode > 0)
                {
                    query = query.Where(task => task.task_status_syscode == searchCriteria.status_syscode);
                }
                if (!string.IsNullOrEmpty(searchCriteria.txtSearch))
                {
                    string likeExprs = $"%{searchCriteria.txtSearch}%";
                    query = query.Where(task =>
                                        DbFunctions.Like(task.task_subject, likeExprs)
                                     || DbFunctions.Like(task.task_reference, likeExprs)
                                     || DbFunctions.Like(task.owner, likeExprs)
                                     || DbFunctions.Like(task.project.project_name, likeExprs)
                                     || DbFunctions.Like(task.module.module_name, likeExprs)
                                     || DbFunctions.Like(task.status.status_name, likeExprs)
                                     || DbFunctions.Like(task.parentTaskRef, likeExprs));
                }
            }
            if (isCreatedByMe)
            {            
                // show if tasks are created by user or user has module write access
                query = query.Where(task => (task.created_by == userId
                              || context.ProjModUserMapping.Any(x=> x.module_syscode == task.module_syscode && x.employee_syscode == userId 
                                        && x.role_syscode == (int)Enum_Master.UserRoleEnum.Module_User && x.access_write
                                        && x.is_active && !x.is_deleted )
                              || context.ProjModUserMapping.Any(x => x.project_syscode == task.project.project_syscode && x.employee_syscode == userId
                                         && x.role_syscode == (int)Enum_Master.UserRoleEnum.Project_User && x.access_write
                                        && x.is_active && !x.is_deleted)
                              )
                              && task.is_deleted == false && task.is_active == true
                              && !(task.task_status_syscode == (int)Enum_Master.StatusEnum.Complete
                                    || task.task_status_syscode == (int)Enum_Master.StatusEnum.Discard)
                              );
                //query = query.Where(task => task.created_by == userId
                //              && task.is_deleted == false && task.is_active == true
                //              && !(task.task_status_syscode == (int)Enum_Master.StatusEnum.Complete
                //                    || task.task_status_syscode == (int)Enum_Master.StatusEnum.Discard)
                //              );
            }
            else if (isOwnedByMe)
            {
                query = query.Where(task => task.task_owner == userId
                              && task.is_deleted == false && task.is_active == true);
            }
            else
            {
                query = query.Where(task => context.TaskUserMapping.Any(x => x.task_syscode == task.task_syscode && x.employee_syscode == userId && x.is_active && !x.is_deleted) //usrTask.employee_syscode == userId
                                  && task.is_deleted == false && task.is_active == true
                                  && (task.task_status_syscode != (int)Enum_Master.StatusEnum.Open)
                                  && (task.task_status_syscode != (int)Enum_Master.StatusEnum.ToDo));
            }

            if (searchCriteria?.task_syscode > 0)
            {
                query = query.OrderBy(x => x.created_on);
            }
            else
            {
                query = query.OrderByDescending(x => x.created_on);
            }

            List<TaskDM> taskList = query.ToList();
            List<TaskDM> taskList1 = taskList.Where(x => x.group_syscode !=null && x.group_syscode != searchCriteria.group_syscode).ToList();

            return taskList.Except(taskList1).ToList();
        }
        public List<TaskDM> getMyCreatedTaskList(int employee_syscode, SearchDTO searchCriteria)
        {
            return getMyTaskList(employee_syscode, searchCriteria, true);
        }
        public List<TaskDM> getMyOwnedTaskList(int employee_syscode, SearchDTO searchCriteria)
        {
            return getMyTaskList(employee_syscode, searchCriteria, false, true);
        }
        public TaskDM GetProjectTaskInfo(JMTask task)
        {
            TaskDM tdm = new TaskDM();
            if (task.module_syscode == null || task.module_syscode == 0)
            {
                tdm.opStatus = false;
                tdm.opMsg = "Module syscode cannot be null or zero for the Project Tasks.";
                return tdm;
            }
            int modSyscode = task.module_syscode ?? 0;
            if (task.task_syscode == 0)
            {
                tdm = (from mdl in context.ModuleMaster
                       where mdl.module_syscode == modSyscode
                       && mdl.is_active && !mdl.is_deleted
                       select new TaskDM
                       {
                           module_syscode = mdl.module_syscode,
                           module = mdl,
                           workflowLevels = (from mod in context.ModuleMaster // just as dummy to facilitate the select
                                             select new ModuleWFLevelMapDM
                                             {
                                                 module_syscode = mdl.module_syscode,
                                                 project_syscode = mdl.project_syscode,
                                                 category_syscode = mdl.category_syscode??0,
                                                 lstLevelTaskUsers = (from mdlLD in context.ModuleLevelDetail
                                                                      join lvl in context.WorkflowLevelDetails on mdlLD.level_syscode equals lvl.level_syscode
                                                                      where mdlLD.is_active && !mdlLD.is_deleted && mdlLD.module_syscode == mdl.module_syscode
                                                                      orderby mdlLD.created_on ascending
                                                                      select new LevelTaskUserDTO
                                                                      {
                                                                          level_name = lvl.level_name,
                                                                          level_syscode = lvl.level_syscode,
                                                                          details_syscode = mdlLD.details_syscode,
                                                                          task_syscode = 0,
                                                                          weightage = mdlLD.weightage,
                                                                          lstUsers = context.ModuleLevelUserMapping.Where(x=> x.details_syscode == mdlLD.details_syscode && x.is_active && !x.is_deleted).Select(x=> x.employee_syscode).ToList()                                                                      
                                                                      }).ToList()
                                             }).FirstOrDefault()
                       }).FirstOrDefault();
            }
            else
            {
                tdm = (from tsk in context.Tasks
                       where tsk.is_active && !tsk.is_deleted && tsk.task_syscode == task.task_syscode
                       select new TaskDM
                       {
                           task_syscode = tsk.task_syscode,
                           module_syscode = tsk.module_syscode,
                           task_subject = tsk.task_subject,
                           task_details = tsk.task_details,
                           target_date = tsk.target_date,
                           task_priority_syscode = tsk.task_priority_syscode,
                           task_on_behalf = tsk.task_on_behalf,
                           task_owner = tsk.task_owner,
                           created_by = tsk.created_by,
                           modified_by = tsk.modified_by,
                           created_on = tsk.created_on,
                           modified_on = tsk.modified_on,
                           weightage = tsk.weightage,
                           task_status_syscode = tsk.task_status_syscode,
                           workflowLevels = (from mdl in context.ModuleMaster
                                             where mdl.module_syscode == tsk.module_syscode
                                             select new ModuleWFLevelMapDM
                                             {
                                                 module_syscode = mdl.module_syscode,
                                                 project_syscode = mdl.project_syscode,
                                                 category_syscode = mdl.category_syscode ?? 0,
                                                 lstLevelTaskUsers = (from steps in context.Tasks                                                                      
                                                                      where steps.is_active && !steps.is_deleted && steps.parent_task_syscode == tsk.task_syscode
                                                                      select new LevelTaskUserDTO
                                                                      {
                                                                          level_name = steps.task_subject,
                                                                          level_syscode = steps.level_syscode ?? 0,
                                                                          level_details = steps.task_details,
                                                                          task_syscode = steps.task_syscode,
                                                                          weightage = steps.weightage,
                                                                          initiate = steps.task_status_syscode == (int)Enum_Master.StatusEnum.Initiate ? true : false,
                                                                          lstUsers = context.TaskUserMapping.Where(x => x.task_syscode == steps.task_syscode && x.is_active && !x.is_deleted).Select(x => x.employee_syscode).ToList()
                                                                      }).ToList()
                                             }).FirstOrDefault()
                       }).FirstOrDefault();
            }
            tdm.workflowLevels.lstLevelTaskUsers.ForEach(x => x.arrUserSyscodes = x.lstUsers.ToArray());
            if (tdm.workflowLevels == null || tdm.workflowLevels.lstLevelTaskUsers == null || tdm.workflowLevels.lstLevelTaskUsers.Count <= 0)
            {
                tdm.blnCreateLevelTasks = false;
            }
            if (tdm.task_status_syscode == (int) Enum_Master.StatusEnum.ToDo)
            {
                tdm.isToDoTask = true;
            }
            return tdm;
        }
        #endregion

        public TaskDM getTaskByID(int task_syscode, int emp_syscode = 0)
        {
            TaskDM taskDM = new TaskDM();
            taskDM = (from tsk in context.Tasks
                      join vwEmp in context.vw_employee_master on tsk.created_by equals vwEmp.employee_syscode
                      join sts in context.TaskStatusMaster on tsk.task_status_syscode equals sts.status_syscode
                      join pri in context.TaskPriorityMaster on tsk.task_priority_syscode equals pri.priority_syscode into taskPriGrp
                      from taskPri in taskPriGrp.DefaultIfEmpty()
                      join cat in context.CategoryMaster on tsk.category_syscode equals cat.category_syscode into taskCatGrp
                      from taskCat in taskCatGrp.DefaultIfEmpty()
                      join mod in context.ModuleMaster on tsk.module_syscode equals mod.module_syscode into taskModGrp
                      from tskmod in taskModGrp.DefaultIfEmpty()
                      join prj in context.ProjectMaster on tskmod.project_syscode equals prj.project_syscode into projModGrp
                      from prjmod in projModGrp.DefaultIfEmpty()
                      join wf in context.WorkflowMaster on tskmod.workflow_syscode equals wf.workflow_syscode into modWFGrp
                      from modWF in modWFGrp.DefaultIfEmpty()
                      join lvl in context.WorkflowLevelDetails on tsk.level_syscode equals lvl.level_syscode into taskLvlGrp
                      from tsklvl in taskLvlGrp.DefaultIfEmpty()                      
                      where tsk.task_syscode == task_syscode && tsk.is_active && !tsk.is_deleted //tskUsr.type_detail == Enum_Master.TaskTypeEnum.Task.ToString()
                      join rec in context.TaskUserRecord on new { col1 = tsk.task_syscode, col2 = emp_syscode } equals new { col1 = rec.task_syscode, col2 = rec.employee_syscode } into recGrp
                      from tskrec in recGrp.Where(r => r.start_time == recGrp.Max(y => y.start_time)).DefaultIfEmpty()
                      select new TaskDM
                      {
                          task_syscode = tsk.task_syscode,
                          task_status_syscode = tsk.task_status_syscode,
                          task_subject = tsk.task_subject,
                          task_reference = tsk.task_reference,
                          target_date = tsk.target_date,
                          task_details = tsk.task_details,
                          task_owner = tsk.task_owner,
                          created_by = tsk.created_by,
                          created_on = tsk.created_on,
                          task_on_behalf = tsk.task_on_behalf,
                          parent_task_syscode = tsk.parent_task_syscode,
                          owner = context.vw_employee_master.Where(x => x.employee_syscode == tsk.task_owner).FirstOrDefault().employee_name,
                          onBehalf = context.vw_employee_master.Where(x => x.employee_syscode == tsk.task_on_behalf).FirstOrDefault().employee_name,
                          strCreatedBy = vwEmp.employee_name,
                          workflow_name = modWF.workflow_name,
                          level_name = tsklvl.level_name,
                          level_syscode = tsklvl.level_syscode,
                          module_syscode = tskmod.module_syscode,
                          module = tskmod,
                          project = prjmod,
                          category = taskCat,
                          taskUserRecord = tskrec,
                          status = sts,
                          priority = taskPri,
                          lstSubtasks = context.Tasks.Where(t => t.parent_task_syscode == tsk.task_syscode && (t.task_status_syscode == (int)Enum_Master.StatusEnum.Open || t.task_status_syscode == (int)Enum_Master.StatusEnum.InProgress || t.task_status_syscode == (int)Enum_Master.StatusEnum.Initiate)).ToList(),
                          lstAttachments = context.TaskAttachment.Where(x => !x.is_deleted && x.type_detail == "Task" && x.type_syscode == tsk.task_syscode).OrderBy(x => x.attachment_syscode).ToList(),
                          lstUsers = context.TaskUserMapping
                                    .Where(x => x.task_syscode == task_syscode && x.is_active && !x.is_deleted)
                                    .Join(context.vw_employee_master, usr => usr.employee_syscode, vwUsrEmp => vwUsrEmp.employee_syscode
                                    , (usr, vwUsrEmp) => new LoginUser
                                    {
                                        employee_name = vwUsrEmp.employee_name
                                    ,
                                        employee_syscode = vwUsrEmp.employee_syscode ?? 0
                                    }).ToList(),
                          lstTrail = context.TaskTrail.Where(t => t.task_syscode == tsk.task_syscode).OrderByDescending(x => x.created_on).ToList()
                      }).FirstOrDefault();
            //Get the activity syscode for activity icon for each trail.
            taskDM.lstTrail.ForEach(x => x.Activity = context.TaskActivityMaster.FirstOrDefault(y => y.activity_syscode == x.activity_syscode));
            //Extract employee syscodes from task members.
            taskDM.arrUserSyscodes = taskDM.lstUsers.Select(x => x.employee_syscode).ToArray();
            taskDM.members = string.Join(", ", taskDM.lstUsers.Select(x => x.employee_name));//.Aggregate("", (accumulator, piece) => accumulator + ", " + piece);
            //Get the tree of the related tasks.
            taskDM.lstTaskTrees = getTaskTree(taskDM.task_syscode, taskDM.module_syscode ?? 0);
            taskDM.startedTaskRefNo = getStartedTaskRefNo(emp_syscode);
            return taskDM;
        }
        public string getStartedTaskRefNo(int emp_syscode)
        {
            string startedTaskRefNo = string.Empty;
            startedTaskRefNo = context.TaskUserRecord.Where(x => x.stop_time == null && x.employee_syscode == emp_syscode)
                                            .Join(context.Tasks, usr => usr.task_syscode, usrtsk => usrtsk.task_syscode
                                            , (usr, usrtsk) => new
                                            {
                                                taskRef = usrtsk.task_reference
                                            }
                                            ).FirstOrDefault()?.taskRef ?? string.Empty;
            return startedTaskRefNo;
        }
        private List<JMTask> getRelatedTaskByID(int caller_task_syscode)
        {
            ///ToDo: Go with the approach of RootParentID and HierarchyID for better performance.
            List<JMTask> lstTasks = new List<JMTask>();
            //(task_syscode, task_reference, task_subject, parent_task_syscode, lvl)
            //tm.task_syscode, tm.task_reference, tm.task_subject, tm.parent_task_syscode, 1 as lvl
            //tm.task_syscode, tm.task_reference, tm.task_subject, tm.parent_task_syscode, p.lvl + 1 as lvl
            int rootTaskSyscode = context.Tasks.SqlQuery
                                    (
                                        @"with Parents
                                        as
                                        (
                                            select tm.*
                                            from task_master tm
                                            where tm.task_syscode = @p0
                                            union all
                                            select tm.*
                                            from task_master tm
                                            join Parents p on tm.task_syscode = p.parent_task_syscode
                                        )
                                        select *
                                        from Parents
                                        where parent_task_syscode is null
                                        option(maxrecursion 0);"
                                    , caller_task_syscode).FirstOrDefault().task_syscode;

            lstTasks = context.Tasks.SqlQuery
                                    (
                                        @"with Parents
                                        as
                                        (
	                                        select tm.*
	                                        from task_master tm
	                                        where tm.task_syscode = @p0
	                                        union all
	                                        select tm.*
	                                        from task_master tm
	                                        join Parents p on tm.parent_task_syscode = p.task_syscode
                                        )
                                        select * 
                                        from Parents
                                        option(maxrecursion 0);"
                                    , rootTaskSyscode).ToList();

            return lstTasks;
        }
        public List<TaskTreeDM> getTaskTree(int caller_task_syscode, int module_syscode)
        {
            List<TaskTreeDM> lst_ttdm = new List<TaskTreeDM>();
            List<JMTask> lstTasks = null;

            if (caller_task_syscode == 0) return lst_ttdm;
            //HashSet<JMTask> hs = new HashSet<JMTask>();

            //if (module_syscode == 0)
            //{
                lstTasks = getRelatedTaskByID(caller_task_syscode);
            //}
            //else
            //{
            //    lstTasks = context.Tasks
            //                                .Where(x => x.module_syscode == module_syscode && !x.is_deleted && x.is_active)
            //                                .OrderBy(x => x.parent_task_syscode)
            //                                .ThenBy(x => x.task_syscode)
            //                                .ToList();
            //}
            string strViewTaskUrl = ConfigurationManager.AppSettings.Get("ViewTaskURL");//context.EmailTemplate.Where(x => x.template_name.Equals("ST_Task_Created") && x.is_active).Select(x => x.link_url).FirstOrDefault();

            var rootTasks = lstTasks.Where(x => x.parent_task_syscode == null || x.parent_task_syscode == 0).ToList();

            for (int i = 0; i < rootTasks.Count; i++)
            {
                JMTask task = rootTasks[i];
                TaskTreeDM taskTreeNode = createNode(task, caller_task_syscode, strViewTaskUrl);
                lst_ttdm.Add(taskTreeNode);
                lstTasks.Remove(task);

                lst_ttdm[i] = returnTree(lst_ttdm[i], lstTasks, caller_task_syscode, strViewTaskUrl);
            }

            //lst_ttdm = returnTree(lstTasks, null, caller_task_syscode, strViewTaskUrl);

            return lst_ttdm;
        }
        private TaskTreeDM returnTree(TaskTreeDM root, List<JMTask> nodes, int caller_task_syscode, string strViewTaskUrl)
        {
            if (nodes.Count == 0) { return root; }

            var children = nodes.Where(x => x.parent_task_syscode == root.task_syscode).ToList();
            if (children != null && children.Count > 0)
            {
                int childCount = children.Count;
                root.nodes = new List<TaskTreeDM>();

                for (int i = 0; i < childCount; i++)
                {
                    JMTask task = children[i];
                    TaskTreeDM taskTreeNode = createNode(task, caller_task_syscode, strViewTaskUrl);
                    root.nodes.Add(taskTreeNode);
                    nodes.Remove(task);
                }

                for (int i = 0; i < childCount; i++)
                {
                    root.nodes[i] = returnTree(root.nodes[i], nodes, caller_task_syscode, strViewTaskUrl);
                    if (nodes.Count == 0) { break; }
                }
            }

            return root;
        }

        private TaskTreeDM createNode(JMTask curTask, int caller_task_syscode, string strViewTaskUrl)
        {
            TaskTreeDM node = null;
            //string strIcon = "<i class=\\\"icon text-danger ion-flag font-size-lg\\\"> </i>";
            node = new TaskTreeDM()
            {
                task_syscode = curTask.task_syscode,
                task_ref = curTask.task_reference,
                parent_syscode = curTask.parent_task_syscode,
                text = curTask.task_subject.Replace("'", "&apos;")//strIcon + 
            };
            if (!string.IsNullOrEmpty(strViewTaskUrl))
                node.href = strViewTaskUrl + ComLibCommon.Base64Encode(curTask.task_syscode + "");

            node.state.expanded = true;
            if (curTask.task_syscode == caller_task_syscode)
            {
                node.state.selected = true;
            }
            return node;
        }

        public TaskDM getTask_SubTaskList(int task_syscode)
        {
            TaskDM taskDM = new TaskDM();
            taskDM = (from tsk in context.Tasks
                      join vwEmp in context.vw_employee_master on tsk.created_by equals vwEmp.employee_syscode
                      join sts in context.TaskStatusMaster on tsk.task_status_syscode equals sts.status_syscode
                      where tsk.task_syscode == task_syscode && tsk.is_active && !tsk.is_deleted
                      select new TaskDM
                      {
                          task_syscode = tsk.task_syscode,
                          task_status_syscode = tsk.task_status_syscode,
                          task_subject = tsk.task_subject,
                          task_reference = tsk.task_reference,
                          strCreatedBy = vwEmp.employee_name,
                          lstSubtasks = context.Tasks.Where(t => t.parent_task_syscode == tsk.task_syscode && t.is_active && !t.is_deleted).ToList(),
                      }).FirstOrDefault();
            return taskDM;
        }

        public int getMaxTrailGroupID(int task_syscode)
        {
            int maxTrailGroupId = (from a in context.TaskTrail
                                   where a.task_syscode == task_syscode && !a.is_deleted
                                   select a.trail_group_id)?.Max() ?? 0;

            return maxTrailGroupId;
        }

        /// <summary>
        /// Get count of modules for tasks created using this workflow
        /// </summary>
        /// <param name="workflow_syscode"></param>
        /// <returns>count of distinct modules where tasks are created</returns>
        public int getCountModuleByWorkflow(int workflow_syscode)
        {
            int cnt = (from a in context.ModuleMaster
                       join b in context.Tasks on a.module_syscode equals b.module_syscode
                       where a.workflow_syscode == workflow_syscode
                       select a.module_syscode).Distinct().Count();

            return cnt;
        }

        public List<TaskDM> getReportTaskList(int employee_syscode, SearchDTO searchCriteria)
        {
            var query = (from task in context.Tasks
                         join sts in context.TaskStatusMaster on task.task_status_syscode equals sts.status_syscode
                         join mod in context.ModuleMaster on new { col1 = task.module_syscode ?? 0, col2 = false, col3 = true } equals new { col1 = mod.module_syscode, col2 = mod.is_deleted, col3 = mod.is_active } into modTaskGrp
                         from modTask in modTaskGrp.DefaultIfEmpty()
                         join proj in context.ProjectMaster on new { col1 = modTask.project_syscode, col2 = false, col3 = true } equals new { col1 = proj.project_syscode, col2 = proj.is_deleted, col3 = proj.is_active } into projModGrp
                         from prjMod in projModGrp.DefaultIfEmpty()
                         //join Map in context.ProjModUserMapping on new { col1 = prjMod.project_syscode, col2 = false, col3 = true } equals new { col1 = Map.project_syscode, col2 = Map.is_deleted, col3 = Map.is_active } into projModMapGrp
                         //from prjMap in projModGrp.DefaultIfEmpty()
                         join vwEmp in context.vw_employee_master on task.task_owner equals vwEmp.employee_syscode into empGrp
                         from g in empGrp.DefaultIfEmpty()
                         join vwEmp1 in context.vw_employee_master on task.modified_by equals vwEmp1.employee_syscode into empGrp1
                         from g1 in empGrp1.DefaultIfEmpty()
                         join p in context.Tasks on task.parent_task_syscode equals p.task_syscode into parentGrp
                         from parent in parentGrp.DefaultIfEmpty()
                         join pri in context.TaskPriorityMaster on task.task_priority_syscode equals pri.priority_syscode into taskPriGrp
                         from taskPri in taskPriGrp.DefaultIfEmpty()
                         
                         select new TaskDM
                         {
                             task_syscode = task.task_syscode,
                             task_reference = task.task_reference,
                             status = sts,
                             group_syscode = prjMod.group_syscode,
                             project = prjMod,
                             module = modTask,
                             task_subject = task.task_subject,
                             owner = g.employee_name,
                             target_date = task.target_date,
                             parent_task_syscode = task.parent_task_syscode,
                             parentTaskRef = parent.task_reference,                            
                             is_deleted = task.is_deleted,
                             is_active = task.is_active,
                             created_by = task.created_by,
                             created_on = task.created_on,
                             task_status_syscode = task.task_status_syscode,
                             weightage = task.weightage,
                             module_syscode = task.module_syscode,
                             level_syscode = task.level_syscode,
                             priority = taskPri,
                             task_owner = task.task_owner,
                             modified_by_Name = g1.employee_name,
                             modified_on = task.modified_on
                         });
            if (searchCriteria != null)
            {
                if (searchCriteria.task_syscode > 0)
                {
                    query = query.Where(task => task.task_syscode == searchCriteria.task_syscode || task.parent_task_syscode == searchCriteria.task_syscode);
                }
                else if (searchCriteria.module_syscode > 0)
                {
                    query = query.Where(task => task.module_syscode == searchCriteria.module_syscode);
                }
                else
                {
                    if (searchCriteria.project_syscode > 0)
                    {
                        query = query.Where(task => task.project.project_syscode == searchCriteria.project_syscode);
                    }
                    if (searchCriteria.category_syscode > 0)
                    {
                        query = query.Where(task => context.ModuleMaster.Where(x => x.category_syscode == searchCriteria.category_syscode).Any(y => y.module_syscode == task.module_syscode));
                    }
                }
                if (searchCriteria.status_syscode > 0)
                {
                    query = query.Where(task => task.task_status_syscode == searchCriteria.status_syscode);
                }
                if (!string.IsNullOrEmpty(searchCriteria.txtSearch))
                {
                    string likeExprs = $"%{searchCriteria.txtSearch}%";
                    query = query.Where(task =>
                                        DbFunctions.Like(task.task_subject, likeExprs)
                                     || DbFunctions.Like(task.task_reference, likeExprs)
                                     || DbFunctions.Like(task.owner, likeExprs)
                                     || DbFunctions.Like(task.project.project_name, likeExprs)
                                     || DbFunctions.Like(task.module.module_name, likeExprs)
                                     || DbFunctions.Like(task.status.status_name, likeExprs)
                                     || DbFunctions.Like(task.parentTaskRef, likeExprs));
                }
            }

            query = query.Where(task => task.is_deleted == false && task.is_active == true
                                && task.task_status_syscode != (int)Enum_Master.StatusEnum.Open
                                && task.task_status_syscode != (int)Enum_Master.StatusEnum.ToDo);

            query = query.Where (task => context.TaskUserMapping.Any(x => x.task_syscode == task.task_syscode && x.employee_syscode == employee_syscode && x.is_active && !x.is_deleted) //usrTask.employee_syscode == userId
                                || context.ProjModUserMapping.Any(x => x.project_syscode == task.project.project_syscode && x.employee_syscode == employee_syscode && x.is_active && !x.is_deleted)
                                || task.created_by == employee_syscode
                                || task.task_owner == employee_syscode
                             );      

            if (searchCriteria.task_syscode > 0)
            {
                query = query.OrderBy(x => x.created_on);
            }
            else
            {
                query = query.OrderByDescending(x => x.created_on);
            }

            List<TaskDM> taskList = query.ToList();
            List<TaskDM> taskList1 = taskList.Where(x => x.group_syscode != null && x.group_syscode != searchCriteria.group_syscode).ToList();

            return taskList.Except(taskList1).ToList();
        }
    }
}

//private List<TaskTreeDM> returnTree(List<JMTask> lstTasks, int? parent_syscode, int caller_task_syscode, string strViewTaskUrl)
//{
//    List<TaskTreeDM> lst_ttdm = new List<TaskTreeDM>();
//    if (lstTasks == null || lstTasks.Count == 0) return null;
//    //int length = lstTasks.Count;

//    //for (int i = 0; i < length; i++)
//    //{
//    int i = 0;
//    while (lstTasks.Count > 0)
//    {
//        JMTask thisTask = lstTasks[i];
//        TaskTreeDM ttdm = createNode(thisTask, caller_task_syscode, strViewTaskUrl);
//        lstTasks.Remove(thisTask);

//        if (ttdm.parent_syscode == parent_syscode || parent_syscode == null)
//        {
//            List<TaskTreeDM> children = returnTree(lstTasks, ttdm.task_syscode, caller_task_syscode, strViewTaskUrl);

//            if (children != null && children.Count > 0)
//            {
//                ttdm.nodes = children;
//            }

//            lst_ttdm.Add(ttdm);
//        }
//    }
//    //}

//    return lst_ttdm;
//}
