using Common_Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_Library.Factories.TaskFactory;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Library.Repository;
using Task_tracker_WebAPI.Areas.Services.Controllers;
using Task_tracker_WebAPI.Controllers;

namespace Task_tracker_WebAPI.Areas.Tasks.Controllers
{
    public class TaskController : BaseAPIController
    {
        [HttpPost]
        public IHttpActionResult MyTasks(TaskUserMapping taskUser)
        {
            List<TaskDM> lst_taskDM = null;
            try
            {
                using (var uow = new UnitOfWork())
                {
                    lst_taskDM = new List<TaskDM>();
                    lst_taskDM = uow.TasksRepo.getMyTaskList(taskUser.employee_syscode, taskUser.searchCriteria);
                    if (lst_taskDM != null && lst_taskDM.Count > 0)
                    {
                        lst_taskDM[0].startedTaskRefNo = uow.TasksRepo.getStartedTaskRefNo(taskUser.employee_syscode);
                        return Ok(lst_taskDM);
                    }
                    else
                    {
                        return Content(HttpStatusCode.NoContent, lst_taskDM);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", taskUser.employee_syscode.ToString(), "MyTasks", "TaskAPIController");
                return Content(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public List<TaskDM> MyCreatedTasks(TaskUserMapping taskUser)
        {
            List<TaskDM> taskDM = null;
            try
            {
                using (var uow = new UnitOfWork())
                {
                    taskDM = new List<TaskDM>();
                    taskDM = uow.TasksRepo.getMyCreatedTaskList(taskUser.employee_syscode, taskUser.searchCriteria);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", taskUser.employee_syscode.ToString(), "MyCreatedTasks", "TaskAPIController");
            }
            return taskDM;
        }

        [HttpPost]
        public List<TaskDM> MyOwnedTasks(TaskUserMapping taskUser)
        {
            List<TaskDM> taskDM = null;
            try
            {
                using (var uow = new UnitOfWork())
                {
                    taskDM = new List<TaskDM>();
                    taskDM = uow.TasksRepo.getMyOwnedTaskList(taskUser.employee_syscode, taskUser.searchCriteria);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", taskUser.employee_syscode.ToString(), "MyOwnedTasks", "TaskAPIController");
            }
            return taskDM;
        }

        [HttpPost]
        public TaskDM ViewTaskByID(TaskDM task)
        {
            int id = task.task_syscode;
            int group_syscode = task.group_syscode?? 0;

            TaskDM taskDM = null;

            if (id == 0)
            {
                throw new Exception("Invalid Task");
            }

            try
            {
                using (var uow = new UnitOfWork())
                {
                    taskDM = uow.TasksRepo.getTaskByID(id, task.logged_in_user);
                    if (taskDM != null)
                    {
                        bool memberAccess = uow.AccessControlRepo.returnProjectAccess(task.logged_in_user, taskDM.project?.project_syscode??0);
                        memberAccess = memberAccess || uow.AccessControlRepo.returnModuleAccess(task.logged_in_user, taskDM.module_syscode ?? 0);
                        taskDM.PageFieldAccesses.Add(nameof(TaskDM.arrUserSyscodes), memberAccess);
                        taskDM.PageHasWriteAccess = uow.AccessControlRepo.returnTaskAccess(task.logged_in_user, id,group_syscode);
                        //Log.LogDebug(JsonConvert.SerializeObject(taskDM.lstTaskTrees), task.logged_in_user + "", task.logged_in_user+"", "Get task by ID", "API Task Controller");
                        taskDM.ddlData = uow.CommonRepo.fillDDLdata(task.ddlData);
                        List<int> disabledValues = ReturnDisabledStatuses(taskDM);

                        taskDM.ddlData.DisabledValues[DBTableNameEnums.TaskStatusMaster] = disabledValues;

                        taskDM.opStatus = true;
                    }
                    else
                    {
                        taskDM = new TaskDM();
                        taskDM.opStatus = false;
                        taskDM.opMsg = "Some error occurred while fetching the task.";
                    }
                }
            }
            catch (Exception ex)
            {
                taskDM = new TaskDM();
                taskDM.opStatus = false;
                taskDM.opMsg = ex.Message;
                taskDM.opInnerException = ex;
                Log.LogError(ex.Message, "", null, "ViewTaskByID", "TaskAPIController");
            }
            return taskDM;
        }

        private static List<int> ReturnDisabledStatuses(TaskDM taskDM)
        {
            ///ToDo: move below business logic to some reasonable place.
            List<int> disabledValues = new List<int>();

            if (taskDM.lstSubtasks.Count > 0)
            {
                disabledValues.Add((int)Enum_Master.StatusEnum.Complete);
                disabledValues.Add((int)Enum_Master.StatusEnum.Discard);
            }
            if (taskDM.task_status_syscode == (int)Enum_Master.StatusEnum.Complete)
            {
                disabledValues.Add((int)Enum_Master.StatusEnum.Discard);
                disabledValues.Add((int)Enum_Master.StatusEnum.Open);
                disabledValues.Add((int)Enum_Master.StatusEnum.InProgress);
                disabledValues.Add((int)Enum_Master.StatusEnum.Initiate);
                disabledValues.Add((int)Enum_Master.StatusEnum.Acknowledge);
                disabledValues.Add((int)Enum_Master.StatusEnum.OnHold);
            }
            else if (taskDM.task_status_syscode == (int)Enum_Master.StatusEnum.Open)
            {
                disabledValues.Add((int)Enum_Master.StatusEnum.Acknowledge);
                disabledValues.Add((int)Enum_Master.StatusEnum.InProgress);
                disabledValues.Add((int)Enum_Master.StatusEnum.Complete);
            }
            else if (taskDM.task_status_syscode == (int)Enum_Master.StatusEnum.Initiate)
            {
                disabledValues.Add((int)Enum_Master.StatusEnum.Open);
                disabledValues.Add((int)Enum_Master.StatusEnum.Complete);
            }

            return disabledValues;
        }

        public JMTask SaveMobileTask(TaskDM taskDM)
        {
            JMTask task = new JMTask();
            //OperationDetailsDTO od = new OperationDetailsDTO();
            try
            {
                task = SaveTaskToDB(taskDM);

                if (task.opStatus)
                {
                    //task.opStatus = true;
                    task.opMsg = "Task created successfully!";
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", taskDM.created_by.ToString(), "SaveTask_Mobile", "TaskAPIController");
                Exception e = ex.ReturnActualException();
                Log.LogDebug(e.Message, "", taskDM.created_by.ToString(), "SaveTask_Mobile", "TaskAPIController");
                task.opStatus = false;
                task.opMsg = "Exception Occurred: " + e.Message;
                task.opInnerException = ex;
                throw;
            }
            return task;
        }

        [HttpPost]
        public JMTask SaveTask()
        {
            JMTask task = null;
            TaskDM taskDM = null;
            try
            {
                string taskVM = HttpContext.Current.Request["ViewModel"];
                taskDM = JsonConvert.DeserializeObject<TaskDM>(taskVM);
                task = taskDM;
                //OperationDetailsDTO od = new OperationDetailsDTO();
                task = SaveTaskToDB(taskDM);

                if (task.opStatus)
                {
                    //task.opStatus = true;
                    task.opMsg = "Task created successfully!";
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", taskDM.created_by.ToString(), "SaveTask", "TaskAPIController");
                Exception e = ex.ReturnActualException();
                Log.LogDebug(e.Message, "", taskDM.created_by.ToString(), "SaveTask", "TaskAPIController");
                task.opStatus = false;
                task.opMsg = "Exception Occurred: " + e.Message;
                task.opInnerException = ex;
            }
            return task;
        }

        private JMTask SaveTaskToDB(TaskDM taskDM)
        {
            JMTask task = null;
            using (var uow = new UnitOfWork())
            {
                string taskRef = "REF_" + String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);
                task = new JMTask();
                task.task_status_syscode = uow.TaskStatusRepo.GetSingle(x => x.status_name == Enum_Master.StatusEnum.Open.ToString()).status_syscode;
                task.created_by = taskDM.created_by;
                task.created_on = DateTime.Now;
                task.is_active = true;
                task.task_reference = taskRef;
                task.task_subject = taskDM.task_subject;
                task.task_details = taskDM.task_details;
                task.task_priority_syscode = taskDM.task_priority_syscode;
                task.category_syscode = taskDM.category_syscode;
                task.task_owner = taskDM.task_owner;
                task.task_on_behalf = taskDM.task_on_behalf;
                task.target_date = taskDM.target_date;

                if (taskDM.isSubTask)
                {
                    task.parent_task_syscode = taskDM.parent_task_syscode;
                    //task.root_parent_task_syscode = uow.TasksRepo.GetSingle(x => x.task_status_syscode == taskDM.parent_task_syscode).root_parent_task_syscode;
                }
                if (taskDM.isWorkflowTask)
                {
                    task.module_syscode = taskDM.module_syscode;
                    task.level_syscode = taskDM.level_syscode;
                }
                uow.TasksRepo.Add(task);
                uow.commitTT();
                //if (task.root_parent_task_syscode == null || task.root_parent_task_syscode == 0)
                //{
                //    task.root_parent_task_syscode = task.task_syscode;
                //}
                //uow.TasksRepo.Update(task);
                //uow.commitTT();

                #region Assign taskdm properties
                taskDM.task_syscode = task.task_syscode;
                taskDM.task_status_syscode = task.task_status_syscode;
                taskDM.task_reference = task.task_reference;              
                #endregion

                #region FileUpload
                FileUpload(task.task_syscode, taskDM.created_by);
                //if (HttpContext.Current.Request.Files.Count > 0)
                //{
                //    //webClient.BaseAddress = new Uri(mongoBaseUri);
                //    FileService fs = new FileService(webClient);
                //    HttpPostedFile file = HttpContext.Current.Request.Files[0];
                //    if (!(string.IsNullOrEmpty(file.FileName) || file.ContentLength == 0))
                //    {
                //        fs.UploadFiles(file, "Task", task.task_syscode, taskDM.created_by);
                //    }
                //}
                #endregion

                // Add Members
                if (taskDM.arrUserSyscodes != null)
                {
                    List<TaskUserMapping> taskUsers = new List<TaskUserMapping>();
                    foreach (var arrItem in taskDM.arrUserSyscodes)
                    {
                        TaskUserMapping user = new TaskUserMapping();
                        user.employee_syscode = arrItem;
                        user.user_role_syscode = (int)Enum_Master.UserRoleEnum.Created_For;
                        user.task_syscode = task.task_syscode;
                        user.created_by = taskDM.created_by;
                        taskUsers.Add(user);
                    }
                    uow.TaskUserMappingRepo.AddRange(taskUsers);
                }

                //Add Task Trail
                TaskTrail tt = new TaskTrail();
                tt.created_by = taskDM.created_by;
                tt.created_on = DateTime.Now;
                tt.task_syscode = task.task_syscode;

                tt.activity_syscode = taskDM.isSubTask ? (int)Enum_Master.ActivityEnum.Created_Subtask : (int)Enum_Master.ActivityEnum.Created;
                tt.trail_description = taskDM.isSubTask ? taskDM.strCreatedBy + " " + Enum_Master.ActivityEnum.Created_Subtask + " On " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") :
                                                          taskDM.strCreatedBy + " " + Enum_Master.ActivityEnum.Created + " Task On " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                uow.TaskTrailRepo.Add(tt);
                uow.commitTT();
                task.opStatus = true;

                #region "Send Email"

                AbstractTask at = FactoryMethod.GetTaskInstance(Enum_Master.TaskType.StandAlone);
                at.Attach(taskDM, Enum_Master.TaskOperationState.Creation, Email_Enums.Email_Type.TC, uow);

               
                bool emailSent = at.BuildEmail().opStatus;
                if (!emailSent)
                {
                    taskDM.opMsg = "eMail could not be sent.";
                }

                //Email_Template temp = uow.EmailTemplateRepo.GetList(x => x.template_name.Equals("ST_Task_Created") && x.is_active).FirstOrDefault();
                //string strMembers = string.Empty;
                //if (temp == null)
                //{
                //    throw new Exception("Email Template Not found.");
                //}

                //List<int> to_syscodes = new List<int>();

                //to_syscodes.Add(taskDM.created_by);

                //if (taskDM.arrUserSyscodes != null)
                //{
                //    to_syscodes.AddRange(taskDM.arrUserSyscodes);
                //    strMembers = uow.CommonRepo.getEmployeeNames(taskDM.arrUserSyscodes);
                //}
                //if (taskDM.arrCCUsersSyscodes != null)
                //{
                //    to_syscodes.AddRange(taskDM.arrCCUsersSyscodes);
                //    strMembers = strMembers + ", " + uow.CommonRepo.getEmployeeNames(taskDM.arrCCUsersSyscodes);
                //}
                //to_syscodes.Add(taskDM.task_owner);
                //to_syscodes.Add(taskDM.task_on_behalf);
                //to_syscodes.RemoveAll(item => item == 0);

                //int template_syscode = temp.template_syscode;
                //string email_subject = temp.template_subject;
                //string email_from_display = temp.from_email_display;
                //string email_from_id = temp.from_email_id;
                //string email_body = temp.template_body;
                //string email_link = temp.link_url + ComLibCommon.Base64Encode(task.task_syscode + "");
                //string email_rows = "<tr><td>" + taskRef + "</td><td>" + taskDM.task_subject +
                //                    "</td><td>" + strMembers + "</td></tr>";

                //email_body = email_body.Replace("#emp_name#", taskDM.strCreatedBy);
                //email_body = email_body.Replace("#activity#", ((Enum_Master.ActivityEnum)tt.activity_syscode).GetDescription().ToString());
                //email_body = email_body.Replace("#Link#", email_link);
                //email_body = email_body.Replace("#rows#", email_rows);

                //bool emailSent = uow.EmailRepo.SendEmail(taskDM.created_by, template_syscode, email_subject, email_from_display, email_from_id, email_body, to_syscodes);

                //if (!emailSent)
                //{
                //    taskDM.opMsg = "eMail could not be sent.";
                //}
                #endregion
            }
            return task;
        }

        [HttpPost]
        public OperationDetailsDTO SaveTrail_Mobile(TaskDM taskdm)
        {
            try
            {
                if (taskdm == null)
                    throw new ArgumentNullException("Task parameter can not be null.");
                taskdm = SaveTrailToDB(taskdm);

                if (taskdm.opStatus)
                {
                    //task.opStatus = true;
                    taskdm.opMsg = "Task created successfully!";
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", taskdm.created_by.ToString(), "SaveTrail_Mobile", "TaskAPIController");
                Exception e = ex.ReturnActualException();
                Log.LogDebug(e.Message, "", taskdm.created_by.ToString(), "SaveTrail_Mobile", "TaskAPIController");
                taskdm.opStatus = false;
                taskdm.opMsg = "Exception Occurred: " + e.Message;
                taskdm.opInnerException = ex;
                throw;
            }
            return taskdm;
        }

        [HttpPost]
        public OperationDetailsDTO SaveTrail()//TaskDM taskdm
        {
            OperationDetailsDTO od = new OperationDetailsDTO();
            TaskDM taskdm = null;
            int group_syscode = 0;

            try
            {
                string taskVM = HttpContext.Current.Request["ViewModel"];
                taskdm = JsonConvert.DeserializeObject<TaskDM>(taskVM);

                if (taskdm != null && taskdm.task_syscode > 0)
                {                   
                    group_syscode = taskdm.group_syscode ?? 0;
                    taskdm = SaveTrailToDB(taskdm);
                }
                else
                {
                    throw new Exception("Task object or Task id is null while saving the trail.");
                }
                return taskdm;
            }
            catch (Exception ex)
            {
                Exception e = ex.ReturnActualException();
                Log.LogError(e.Message, "", taskdm.created_by.ToString(), "SaveTrail", "TaskAPIController");
                od.opStatus = false;
                od.opMsg = "Exception Occurred! Exception: " + e.Message;
                od.opInnerException = e;
                ///ToDo: Find better approach for doing this patching work, for getting the previous data as it is.
                using (var uow = new UnitOfWork())
                {
                    DDLDTO ddlData = taskdm.ddlData;
                    taskdm = uow.TasksRepo.getTaskByID(taskdm.task_syscode, taskdm.logged_in_user);
                    taskdm.PageHasWriteAccess = uow.AccessControlRepo.returnTaskAccess(taskdm.logged_in_user, taskdm.task_syscode, group_syscode);
                    taskdm.ddlData = uow.CommonRepo.fillDDLdata(ddlData);
                    taskdm.opInnerException = e;
                    taskdm.opMsg = e.Message;
                    taskdm.opStatus = true;
                    return taskdm;
                }

            }
            //return od;
        }

        private void SaveReleaseDetails(TaskDM taskdm)
        {
            if (!string.IsNullOrEmpty(taskdm.releaseDetailsJson))
            {
                var relInst = JsonConvert.DeserializeObject<ReleaseInstructions>(taskdm.releaseDetailsJson);

                if (relInst != null && relInst.env_syscode > 0 && relInst.project_syscode > 0 && relInst.task_syscode > 0 && relInst.lstReleaseDetails != null && relInst.lstReleaseDetails.Count > 0)
                {
                    using (var uow = new UnitOfWork())
                    {
                        //Save Release Instruction
                        int cnt = 0;
                        var lst = uow.ReleaseInstRepo.GetList(r => r.task_syscode == relInst.task_syscode 
                                                                    && r.env_syscode == relInst.env_syscode
                                                                    && r.is_active
                                                                    && !r.is_deleted
                                                                    ).ToList();
                        if (lst != null && lst.Count() > 0)
                            cnt = lst.Count;

                        cnt = ++cnt;
                        relInst.release_ref = $"RL{cnt}_{DateTime.Now.ToString("yyyy_MM_dd_HH:mm:ss")}";
                        relInst.created_by = taskdm.logged_in_user;
                        uow.ReleaseInstRepo.Add(relInst);
                        uow.commitTT();

                        List<ReleaseDetails> lstRelDetails = new List<ReleaseDetails>();
                        foreach (var item in relInst.lstReleaseDetails)
                        {
                            ReleaseDetails rd = new ReleaseDetails();
                            rd.release_syscode = relInst.release_syscode;
                            rd.parameter_syscode = item.parameter_syscode;
                            rd.parameter_value = item.parameter_value;
                            rd.created_by = taskdm.logged_in_user;
                            rd.is_active = true;
                            rd.is_deleted = false;
                            lstRelDetails.Add(rd);
                        }
                        uow.ReleaseDetailsRepo.AddRange(lstRelDetails);
                        uow.commitTT();
                    }
                } 
            }
        }
        private TaskDM SaveTrailToDB(TaskDM taskdm)
        {
            using (var uow = new UnitOfWork())
            {

                ///ToDo: Find better approach to handle this.
                #region Assign taskdm properties
                var taskById = uow.TasksRepo.getTaskByID(taskdm.task_syscode);

                taskdm.module = taskById.module;
                taskdm.project = taskById.project;
                taskdm.category = uow.CategoryRepo.GetSingle(x => x.category_syscode == taskById.module?.category_syscode);
                taskdm.workflow_name = taskById.workflow_name;
                taskdm.level_name = taskById.level_name;
                taskdm.parent_task_syscode = taskById.parent_task_syscode;
                #endregion

                AbstractTask at = FactoryMethod.GetTaskInstance(taskdm.module_syscode > 0 ? Enum_Master.TaskType.Workflow : Enum_Master.TaskType.StandAlone);
                at.Attach(taskdm, Enum_Master.TaskOperationState.Updation, Email_Enums.Email_Type.TU, uow);
                at.InitialiseTrail();
                at.Process();

                //if (taskdm.lstTrail.Count > 0)
                //{

                //string strComments = "";

                ////Code to group trail records ----
                //int max_trail_group = uow.TasksRepo.getMaxTrailGroupID(taskdm.task_syscode);
                //taskdm.lstTrail.ForEach(t => { t.trail_group_id = max_trail_group + 1; });
                ////--------------



                //uow.TaskTrailRepo.AddRange(taskdm.lstTrail);

                //#region "Comments Added"
                //if (taskdm.lstTrail.Any(t => t.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments))
                //{
                //    strComments = taskdm.lstTrail.FirstOrDefault(x => x.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments).trail_comments;
                //}

                //#endregion

                #region "Status Changed"
                //This code has been moved to the factory.                

                #endregion

                #region "Member Added"
                at.UpdateMembers();
                //This code has been moved to task factory.

                #endregion

                #region FileUpload
                var UpFileNames = FileUpload(taskdm.task_syscode, taskdm.created_by, true);

                if (UpFileNames.Count > 0)
                    at.CreateTrail(Enum_Master.ActivityEnum.Added_File, UpFileNames.ToArray());
                #endregion

                #region Adding Trail
                at.AddTrails();
                #endregion


                #region "Save Release Details"

                SaveReleaseDetails(taskdm);
                #endregion

                #region Should be removed when redirection is implemented upon successful save.
                DDLDTO ddldata = taskdm.ddlData;
                int loggedInUser = taskdm.logged_in_user;
                taskdm = uow.TasksRepo.getTaskByID(taskdm.task_syscode, taskdm.created_by == 0 ? taskdm.logged_in_user : taskdm.created_by);
                taskdm.logged_in_user = loggedInUser;
                taskdm.PageHasWriteAccess = uow.AccessControlRepo.returnTaskAccess(taskdm.logged_in_user, taskdm.task_syscode, taskdm.group_syscode??0);
                taskdm.ddlData = uow.CommonRepo.fillDDLdata(ddldata);
                List<int> disabledValues = ReturnDisabledStatuses(taskdm);
                taskdm.ddlData.DisabledValues[DBTableNameEnums.TaskStatusMaster] = disabledValues;

                taskdm.task_comment = string.Empty;
                #endregion
                taskdm.opStatus = true;

                #region "Send eMail"

                at.BuildEmail(); 

                #endregion
                //}
            }
            return taskdm;
        }

        

        [HttpPost]
        public TaskUserRecord SaveTaskStart(TaskUserRecord record)
        {
            try
            {
                using (var uow = new UnitOfWork())
                {
                    record.start_time = DateTime.Now;
                    uow.TaskUserRecordRepo.Add(record);

                    //Get all open tasks for this user
                    List<TaskUserRecord> lstTaskUserRecordsinDB = uow.TaskUserRecordRepo
                                                                   .GetList(x => x.employee_syscode.Equals(record.employee_syscode)
                                                                            && x.stop_time == null)
                                                                   ?.ToList();

                    if (lstTaskUserRecordsinDB != null && lstTaskUserRecordsinDB.Count > 0)
                    {
                        if (lstTaskUserRecordsinDB.Any(x => x.task_syscode == record.task_syscode))
                            throw new InvalidOperationException("This task has been already started by you!");

                        lstTaskUserRecordsinDB.ForEach(x => { x.stop_time = DateTime.Now; });
                        uow.TaskUserRecordRepo.UpdateRange(lstTaskUserRecordsinDB);
                    }


                    // Change task status to InProgress if in Initite status. Trail not created
                    JMTask objTask = uow.TasksRepo.Get(record.task_syscode);
                    if (objTask.task_status_syscode == (int)Enum_Master.StatusEnum.Initiate || objTask.task_status_syscode == (int)Enum_Master.StatusEnum.Acknowledge)
                    {
                        objTask.task_status_syscode = (int)Enum_Master.StatusEnum.InProgress;
                        objTask.modified_by = record.employee_syscode;
                        objTask.modified_on = DateTime.Now;
                        uow.TasksRepo.Update(objTask);
                    }

                    uow.commitTT();
                    record = uow.TaskUserRecordRepo.GetList(x =>
                                            x.task_syscode == record.task_syscode
                                         && x.employee_syscode == record.employee_syscode)
                                         .OrderByDescending(x => x.record_syscode).Take(1).FirstOrDefault();

                    // Change task status to InProgress if not already. Trail Created
                    //TaskDM taskdm = uow.TasksRepo.getTaskByID(record.task_syscode);
                    //if (taskdm.task_status_syscode != (int)Enum_Master.StatusEnum.InProgress)
                    //{

                    //    //taskdm.task_syscode = objTask.task_syscode;
                    //    taskdm.task_comment = "Task Started";
                    //    taskdm.task_status_syscode = (int)Enum_Master.StatusEnum.InProgress;
                    //    taskdm.modified_by = record.employee_syscode;
                    //    taskdm.modified_on = DateTime.Now;
                    //    SaveTrailToDB(taskdm);
                    //}
                    record.opStatus = true;
                    record.opMsg = "Success";
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", record.employee_syscode.ToString(), "SaveTaskStart", "TaskAPIController");
                record.opStatus = false;
                record.opMsg = ex.Message;
                record.opInnerException = ex.InnerException;
            }
            return record;
        }


        [HttpPost]
        public TaskUserRecord SaveTaskStop(TaskUserRecord userTaskRec)
        {
            try
            {
                using (var uow = new UnitOfWork())
                {
                    TaskUserRecord rec = uow.TaskUserRecordRepo.GetList(x =>
                                            x.task_syscode == userTaskRec.task_syscode
                                         && x.employee_syscode == userTaskRec.employee_syscode
                                         && x.stop_time == null
                                         && x.start_time != null).FirstOrDefault();
                    if (rec != null)
                    {
                        rec.stop_time = DateTime.Now;
                        uow.TaskUserRecordRepo.Update(rec);
                        uow.commitTT();

                        rec = uow.TaskUserRecordRepo.GetList(x =>
                                            x.task_syscode == userTaskRec.task_syscode
                                         && x.employee_syscode == userTaskRec.employee_syscode)
                                         .OrderByDescending(x => x.record_syscode).Take(1).FirstOrDefault();

                        userTaskRec = rec;
                        userTaskRec.opStatus = true;
                        userTaskRec.opMsg = "Success";
                    }
                    else
                    {
                        userTaskRec.opStatus = false;
                        userTaskRec.opMsg = "This task has been not yet started or already stopped by you.";
                    }

                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", userTaskRec.employee_syscode.ToString(), "SaveTaskStop", "TaskAPIController");
                userTaskRec.opStatus = false;
                userTaskRec.opMsg = "Exception Occurred!";
                userTaskRec.opInnerException = ex.InnerException;
            }
            return userTaskRec;
        }


        [HttpPost]
        public TaskDM GetTask_SubTasks(TaskDM task)
        {
            TaskDM taskDM = null;
            try
            {
                int id = task.task_syscode;

                if (id == 0)
                {
                    throw new Exception("Invalid Task");
                }

                using (var uow = new UnitOfWork())
                {
                    taskDM = uow.TasksRepo.getTask_SubTaskList(id);
                    if (taskDM != null)
                    {
                        taskDM.opStatus = true;
                    }
                    else
                    {
                        taskDM = new TaskDM();
                        taskDM.opStatus = false;
                        taskDM.opMsg = "Error occurred while fetching the task & subtasks.";
                    }
                }
            }
            catch (Exception ex)
            {
                taskDM = new TaskDM();
                taskDM.opStatus = false;
                taskDM.opMsg = ex.Message;
                taskDM.opInnerException = ex;
                Log.LogError(ex.Message, "", null, "GetTask_SubTasks", "TaskAPIController");
            }
            return taskDM;
        }

        [HttpPost]
        public TaskDM SaveWeightage(TaskDM taskDM)
        {
            string email_rows = "";

            if (taskDM.lstSubtasks.Count <= 0)
                throw new ArgumentNullException("lstSubtasks", "List of tasks can not be empty.");

            try
            {
                OperationDetailsDTO od = new OperationDetailsDTO();
                using (var uow = new UnitOfWork())
                {
                    List<int> to_syscodes = new List<int>();
                    List<int> memberSyscodes = new List<int>();
                    int linkCounter = 0;

                    string logged_in_user = uow.EmployeeRepo.Get(taskDM.created_by).employee_name;

                    #region "Get Email Template"
                    EmailTemplate temp = uow.EmailTemplateRepo.GetList(x => x.template_name.Equals("Weightage_Added") && x.is_active).FirstOrDefault();
                    if (temp == null)
                    {
                        throw new Exception("Email Template Not found.");
                    }
                    int template_syscode = temp.template_syscode;
                    string email_subject = temp.template_subject;
                    string email_from_display = temp.from_email_display;
                    string email_from_id = temp.from_email_id;
                    string email_body = temp.template_body;

                    #endregion

                    foreach (var tsk in taskDM.lstSubtasks)
                    {
                        JMTask objTask = null;

                        if (tsk.weightage <= 0 || tsk.task_syscode == 0)
                        {
                            continue;
                        }

                        objTask = uow.TasksRepo.Get(tsk.task_syscode);
                        if (objTask.weightage == tsk.weightage)
                        {
                            //continue;
                        }

                        objTask.weightage = tsk.weightage;
                        objTask.modified_by = taskDM.created_by;
                        objTask.modified_on = DateTime.Now;

                        uow.TasksRepo.Update(objTask);
                        uow.commitTT();

                        //Add Task Trail
                        string desc = Enum_Master.ActivityEnum.Changed_Weightage.GetDescription();

                        TaskTrail tt = new TaskTrail();
                        tt.created_by = taskDM.created_by;
                        tt.created_on = DateTime.Now;
                        tt.task_syscode = objTask.task_syscode;
                        tt.activity_syscode = (int)Enum_Master.ActivityEnum.Changed_Weightage;
                        tt.trail_description = taskDM.strCreatedBy + " " + desc + " On " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        tt.trail_comments = desc;
                        uow.TaskTrailRepo.Add(tt);

                        uow.commitTT();
                        taskDM.opStatus = true;
                        taskDM.opMsg = "Record updated sucessfully.";


                        //Get Task Members
                        List<TaskUserMapping> TaskUserMappingsinDB = uow.TaskUserMappingRepo
                                                                     .GetList(x => x.task_syscode.Equals(objTask.task_syscode)
                                                                                 && !x.is_deleted
                                                                                 && x.is_active)
                                                                     ?.ToList();
                        TaskUserMappingsinDB.ForEach(x => { to_syscodes.Add(x.employee_syscode); memberSyscodes.Add(x.employee_syscode); });

                        to_syscodes.Add(objTask.created_by);
                        to_syscodes.Add(taskDM.created_by);
                        to_syscodes.Add(objTask.task_owner);
                        to_syscodes.Add(objTask.task_on_behalf);
                        to_syscodes.RemoveAll(item => item == 0);


                        string strMembers = string.Empty;
                        if (memberSyscodes != null)
                            strMembers = uow.CommonRepo.getEmployeeNames(memberSyscodes);

                        string email_link = "";
                        if (linkCounter == 0)
                        {
                            email_link = temp.link_url + "?returnValue=Tasks/Task/ViewTask/" + ComLibCommon.Base64Encode(objTask.task_syscode + "");
                        }
                        else
                        {
                            email_link = temp.link_url + "?returnValue" + linkCounter + "=Tasks/Task/ViewTask/" + ComLibCommon.Base64Encode(objTask.task_syscode + "");
                        }

                        email_rows += "<tr><td><a href=\"" + email_link + "\">" + objTask.task_reference + "</a></td><td>" + objTask.task_subject + "</td><td>" + strMembers + "</td></tr>";
                        linkCounter = linkCounter + 1;
                    }

                    #region "Send Email"

                    email_body = email_body.Replace("#comments#", string.Empty);
                    email_body = email_body.Replace("#emp_name#", logged_in_user);
                    email_body = email_body.Replace("#activity#", "Weightage Added.");
                    email_body = email_body.Replace("#rows#", email_rows);
                    email_from_display = email_from_display.Replace("#emp_name#", logged_in_user);

                    to_syscodes.RemoveAll(item => item == 0);
                    taskDM.opStatus = true;

                    bool emailSent = uow.EmailRepo.SendEmail(taskDM.created_by, template_syscode, email_subject, email_from_display, email_from_id, email_body, to_syscodes);
                    if (!emailSent)
                    {
                        taskDM.opMsg = "eMail could not be sent.";
                    }

                    //var taskById = uow.TasksRepo.getTaskByID(taskDM.task_syscode);

                    //taskDM.module = taskById.module;
                    //taskDM.project = taskById.project;
                    //taskDM.category = uow.CategoryRepo.GetSingle(x => x.category_syscode == taskById.module?.category_syscode);
                    //taskDM.workflow_name = taskById.workflow_name;
                    //taskDM.level_name = taskById.level_name;
                    //taskDM.task_comment = "Weightage Added";

                    //AbstractTask at = FactoryMethod.GetTaskInstance(Enum_Master.TaskType.Workflow);
                    //at.Attach(taskDM, Enum_Master.TaskOperationState.Updation, Email_Enums.Email_Type.TU, uow);


                    //bool emailSent = at.BuildEmail().opStatus;
                    //if (!emailSent)
                    //{
                    //    taskDM.opMsg = "eMail could not be sent.";
                    //}
                    #endregion

                }
            }
            catch (Exception ex)
            {
                taskDM.opStatus = false;
                taskDM.opMsg = ex.Message;
                taskDM.opInnerException = ex;
                Log.LogError(ex.Message, "", null, "SaveWeightage", "TaskAPIController");
            }
            return taskDM;
        }

        [HttpPost]
        public List<TaskDM> GetTaskReport(TaskUserMapping taskUser)
        {
            List<TaskDM> taskDM = null;
            try
            {
                using (var uow = new UnitOfWork())
                {
                    taskDM = new List<TaskDM>();
                    taskDM = uow.TasksRepo.getReportTaskList(taskUser.employee_syscode, taskUser.searchCriteria);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", taskUser.employee_syscode.ToString(), "MyCreatedTasks", "TaskAPIController");
            }
            return taskDM;
        }


      

    }
}
