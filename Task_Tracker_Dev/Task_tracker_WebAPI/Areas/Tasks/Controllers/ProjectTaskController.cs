using Common_Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Library.Factories.TaskFactory;
using Task_Tracker_Library.Repository;
using Task_tracker_WebAPI.Controllers;

namespace Task_tracker_WebAPI.Areas.Tasks.Controllers
{
    public class ProjectTaskController : BaseAPIController
    {
        // GET: Tasks/ProjectTask
        [HttpPost]
        public IHttpActionResult GetProjectTaskInfo(TaskDM _taskDm)
        {
            TaskDM tdm = new TaskDM();
            tdm.ddlData = _taskDm.ddlData;

            try
            {
                using (var uow = new UnitOfWork())
                {
                    if (_taskDm.module_syscode == null || _taskDm.module_syscode == 0)
                    {
                        tdm.opStatus = false;
                        tdm.opMsg = "Module syscode can not be 0 or null for the project tasks.";
                        tdm.ddlData = uow.CommonRepo.fillDDLdata(_taskDm.ddlData);
                        return Ok(tdm);
                    }
                    tdm = uow.TasksRepo.GetProjectTaskInfo(_taskDm);
                    if (tdm != null)
                    {
                        tdm.ddlData = uow.CommonRepo.fillDDLdata(_taskDm.ddlData);
                        tdm.PageHasWriteAccess = uow.AccessControlRepo.returnModuleAccess(_taskDm.logged_in_user, _taskDm.module_syscode ?? 0)
                                        || uow.AccessControlRepo.returnProjectAccess(_taskDm.logged_in_user, _taskDm.project.project_syscode);
                        return Ok(tdm);
                    }
                    else
                    {
                        return Content(HttpStatusCode.NoContent, tdm);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", _taskDm.logged_in_user.ToString(), "GetProjectTaskInfo", "ProjectTaskController");
                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public IHttpActionResult SaveProjectTask()
        {
            JMTask task = null;
            TaskDM taskDM = null;
            try
            {
                string taskVM = HttpContext.Current.Request["ViewModel"];
                taskDM = JsonConvert.DeserializeObject<TaskDM>(taskVM);

                //OperationDetailsDTO od = new OperationDetailsDTO();
                task = SaveTaskToDB(taskDM);

                if (task.opStatus)
                {
                    task.opStatus = true;
                    task.opMsg = "Task created successfully!";
                    return Ok(task);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", taskDM.created_by.ToString(), "SaveProjectTask", "ProjectTaskController");
                Exception e = ex.ReturnActualException();
                Log.LogDebug(e.Message, "", taskDM.created_by.ToString(), "SaveProjectTask", "ProjectTaskController");
                task.opStatus = false;
                task.opMsg = "Exception Occurred: " + e.Message;
                task.opInnerException = ex;
                return InternalServerError(ex);
            }
            //return task;
            return Content(HttpStatusCode.InternalServerError, "Problem occurred while processing the request, Task not Saved.");
        }

        private JMTask SaveTaskToDB(TaskDM taskDM)
        {
            JMTask task = null;
            using (var uow = new UnitOfWork())
            {
                if (taskDM.task_syscode > 0)
                {
                    return updateTask(taskDM, uow);
                }

                string taskRef = "REF_" + String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);
                string statusName = Enum_Master.StatusEnum.Open.ToString();
                if (taskDM.isToDoTask)
                {
                    statusName = Enum_Master.StatusEnum.ToDo.ToString();
                }
                else if (taskDM.workflowLevels.lstLevelTaskUsers != null && taskDM.workflowLevels.lstLevelTaskUsers.Any(x => x.initiate))
                {
                    statusName = Enum_Master.StatusEnum.Initiate.ToString();
                }

                task = new JMTask();
                task.task_status_syscode = uow.TaskStatusRepo.GetSingle(x => x.status_name == statusName).status_syscode;
                task.created_by = taskDM.created_by;
                task.created_on = DateTime.Now;
                task.is_active = true;
                task.task_reference = taskRef;
                task.task_subject = taskDM.task_subject;
                task.task_details = taskDM.task_details;
                task.task_priority_syscode = taskDM.task_priority_syscode;
                task.weightage = 100;
                //task.category_syscode = taskDM.category_syscode;
                task.task_owner = taskDM.task_owner;
                task.task_on_behalf = taskDM.task_on_behalf;
                task.target_date = taskDM.target_date;

                task.module_syscode = taskDM.module_syscode;

                uow.TasksRepo.Add(task);
                uow.commitTT();

                #region Assign taskdm properties
                taskDM.task_syscode = task.task_syscode;
                taskDM.task_status_syscode = task.task_status_syscode;
                taskDM.task_reference = task.task_reference;
                var taskById = uow.TasksRepo.getTaskByID(task.task_syscode);

                taskDM.module = taskById.module;
                taskDM.project = taskById.project;
                taskDM.category = uow.CategoryRepo.GetSingle(x=> x.category_syscode == taskById.module.category_syscode);
                taskDM.workflow_name = taskById.workflow_name;
                #endregion

                #region FileUpload
                FileUpload(task.task_syscode, taskDM.created_by);
                #endregion

                // Add Members  
                //currently we are not adding members for root task so that the mail does not go to all the people.
                //This change came due to requirement in email changes.              
                //taskDM.arrUserSyscodes = (from users in uow.ProjModUserMappingRepo.GetList(x => x.module_syscode == taskDM.module_syscode && x.is_active && !x.is_deleted)
                //                          select users.employee_syscode).ToArray();
                //AddTaskMembers(taskDM.arrUserSyscodes, task, uow);

                //Add Task Trail
                TaskTrail tt = CreateTaskTrail(taskDM, task, uow);
                uow.commitTT();

                AbstractTask at = FactoryMethod.GetTaskInstance(Enum_Master.TaskType.Workflow);
                at.Attach(taskDM, Enum_Master.TaskOperationState.Creation, Email_Enums.Email_Type.TC, uow);
                
                #region "Send Email For Project task creation"     
                bool emailSent = at.BuildEmail().opStatus;
                if (!emailSent)
                {
                    taskDM.opMsg = "eMail could not be sent.";
                }
                #endregion

                #region Creating tasks for steps
                if (taskDM.blnCreateLevelTasks)
                {
                    CreateStepTasks(taskDM, uow);
                }
                #endregion

                task.opStatus = true;
            }
            return task;
        }

        private JMTask updateTask(TaskDM taskDM, UnitOfWork uow)
        {
            JMTask task = uow.TasksRepo.GetSingle(x => x.task_syscode == taskDM.task_syscode);

            string statusName = taskDM.isToDoTask ? Enum_Master.StatusEnum.ToDo.ToString() : Enum_Master.StatusEnum.Open.ToString();
            task.task_status_syscode = uow.TaskStatusRepo.GetSingle(x => x.status_name == statusName).status_syscode;
            task.task_subject = taskDM.task_subject;
            task.task_details = taskDM.task_details;
            task.task_owner = taskDM.task_owner;
            task.task_on_behalf = taskDM.task_on_behalf;
            task.task_priority_syscode = taskDM.task_priority_syscode;
            uow.TasksRepo.Update(task);
            uow.commitTT();
            task.opStatus = true;
            return task;
        }

        private static TaskTrail CreateTaskTrail(TaskDM taskDM, JMTask task, UnitOfWork uow)
        {
            TaskTrail tt = new TaskTrail();
            tt.created_by = taskDM.created_by;
            tt.created_on = DateTime.Now;
            tt.task_syscode = task.task_syscode;

            tt.activity_syscode = (int)Enum_Master.ActivityEnum.Created;
            tt.trail_description = taskDM.strCreatedBy + " " + Enum_Master.ActivityEnum.Created + " Task On " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            uow.TaskTrailRepo.Add(tt);
            return tt;
        }

        private static void AddTaskMembers(int[] _arrUsrSyscodes, JMTask task, UnitOfWork uow)
        {
            if (_arrUsrSyscodes != null)
            {
                List<TaskUserMapping> taskUsers = new List<TaskUserMapping>();
                foreach (var arrItem in _arrUsrSyscodes)
                {
                    TaskUserMapping user = new TaskUserMapping();
                    user.employee_syscode = arrItem;
                    user.user_role_syscode = (int)Enum_Master.UserRoleEnum.Created_For;
                    user.task_syscode = task.task_syscode;
                    user.created_by = task.created_by;
                    taskUsers.Add(user);
                }
                uow.TaskUserMappingRepo.AddRange(taskUsers);
            }
        }

        private static void CreateStepTasks(TaskDM taskDM, UnitOfWork uow)
        {
            if (taskDM.workflowLevels != null && taskDM.workflowLevels.lstLevelTaskUsers != null)
            {
                foreach (var step in taskDM.workflowLevels.lstLevelTaskUsers)
                {
                    string taskRef = "REF_" + String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);
                    string statusName = taskDM.isToDoTask ? Enum_Master.StatusEnum.ToDo.ToString() : (step.initiate ? Enum_Master.StatusEnum.Initiate.ToString() : Enum_Master.StatusEnum.Open.ToString());
                    JMTask stpTask = new JMTask();
                    stpTask.task_status_syscode = uow.TaskStatusRepo.GetSingle(x => x.status_name == statusName).status_syscode;
                    stpTask.created_by = taskDM.created_by;
                    stpTask.created_on = DateTime.Now;
                    stpTask.is_active = true;
                    stpTask.task_reference = taskRef;
                    stpTask.task_subject = step.level_name;
                    stpTask.task_details = step.level_details;
                    //stpTask.task_priority_syscode = Enum_Master.PriorityEnum.;
                    stpTask.category_syscode = taskDM.category_syscode;
                    stpTask.task_owner = taskDM.task_owner;
                    stpTask.task_on_behalf = taskDM.task_on_behalf;
                    stpTask.target_date = step.target_date;
                    stpTask.weightage = step.weightage ?? 0;

                    stpTask.module_syscode = taskDM.module_syscode;
                    stpTask.level_syscode = step.level_syscode;
                    stpTask.parent_task_syscode = taskDM.task_syscode;

                    uow.TasksRepo.Add(stpTask);
                    uow.commitTT();

                    ///ToDo: Find better approach for handling this level name thing.
                    taskDM.level_name = step.level_name;

                    AddTaskMembers(step.arrUserSyscodes, stpTask, uow);
                    uow.commitTT();
                    TaskTrail tt = CreateTaskTrail(taskDM, stpTask, uow);
                    uow.commitTT();

                    AbstractTask at = FactoryMethod.GetTaskInstance(Enum_Master.TaskType.Workflow);
                    at.Attach(taskDM, Enum_Master.TaskOperationState.Creation, Email_Enums.Email_Type.TC, uow);                    

                    bool emailSent = at.BuildEmail(stpTask, step.arrUserSyscodes).opStatus;
                    if (!emailSent)
                    {
                        taskDM.opMsg = "eMail could not be sent.";
                    }

                    //if (step.initiate && !taskDM.isToDoTask)
                    //{
                    //    SendEmail(taskDM, stpTask, uow, tt, step.arrUserSyscodes);
                    //}
                }
            }
        }        
    }
}