using Common_Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Solution.Areas.Common.Controllers;
using Task_Tracker_Solution.Areas.Master.Models;
using Task_Tracker_Solution.Areas.Tasks.Models;
using Task_Tracker_Solution.Utility;

namespace Task_Tracker_Solution.Areas.Tasks.Controllers
{
    public class TaskController : TaskTrackerBaseController
    {
        public ActionResult MyTasks(SearchDTO _searchCriteria)
        {
            TaskViewModel viewmodel = null;
            try
            {
                _searchCriteria = FillSearchDTO(_searchCriteria);
                
                viewmodel = new TaskViewModel();                               
                _searchCriteria.group_syscode = ssGroupSyscode;
                viewmodel.searchDTO = _searchCriteria;

                TaskUserMapping taskUser = new TaskUserMapping();
                taskUser.employee_syscode = ssLoggedInEmpSyscode;
                taskUser.searchCriteria = _searchCriteria;

                var response = client.PostAsJsonAsync(cWebApiNames.APIGetMyTasks, taskUser).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    viewmodel.lstTaskDM = JsonConvert.DeserializeObject<List<TaskDM>>(responseMsg);
                    viewmodel.searchDTO = _searchCriteria;
                    //if (viewmodel.lstTaskDM != null && viewmodel.lstTaskDM.Count > 0)
                    //{///ToDo: Ask Aartiji logic behind this //We agreed to remove it.
                    //    viewmodel.module.module_description = viewmodel.lstTaskDM.First()?.module?.module_description; 
                    //}
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "MyTasks", "TaskController");
            }

            return View(viewmodel);
        }

        public ActionResult MyCreatedTasks(SearchDTO _searchCriteria)
        {
            TaskViewModel viewmodel = null;
            try
            {
                _searchCriteria = FillSearchDTO(_searchCriteria);
                _searchCriteria.group_syscode = ssGroupSyscode;

                viewmodel = new TaskViewModel();
                viewmodel.searchDTO = _searchCriteria;

                TaskUserMapping tum = new TaskUserMapping();
                tum.employee_syscode = ssLoggedInEmpSyscode;
                tum.searchCriteria = _searchCriteria;

                var response = client.PostAsJsonAsync(cWebApiNames.APIGetMyCreatedTasks, viewmodel).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    var tasks = JsonConvert.DeserializeObject<List<TaskDM>>(responseMsg);

                    #region Re-Arranging the tasks based on parent child relation 
                    List<TaskDM> lstTasks = new List<TaskDM>();
                    List<TaskDM> subTasks = new List<TaskDM>();
                    foreach (var task in tasks)
                    {
                        //Adding the Standalone task.
                        if (task.module_syscode == null || task.module_syscode == 0)
                            lstTasks.Add(task);
                        else
                        {
                            if (task.parent_task_syscode == null || task.parent_task_syscode == 0)
                            {
                                lstTasks.Add(task);
                                //Below logic considers that tasks would come ordered in descending order of creation time and date.
                                ///ToDo: Low: Extreme condition: Might occure at very later point of the system life; This below logic might not work when the order of the tasks is not proper in case of 
                                /// Concurrent task creation by multiple users simultaneously. In this case task creation in the Web API
                                /// needs to have a lock.
                                /// Grand Child tasks are not visible by doing this.
                                subTasks = tasks.Where(x=> x.parent_task_syscode == task.task_syscode).OrderBy(x => x.created_on).ToList();
                                lstTasks.AddRange(subTasks);
                                subTasks = new List<TaskDM>();
                            }
                            //else
                            //{
                            //    subTasks.Add(task);
                            //}
                        }
                    } 
                    #endregion

                    viewmodel.lstTaskDM = lstTasks;
                    viewmodel.searchDTO = _searchCriteria;
                    viewmodel.module.module_description = viewmodel.lstTaskDM.First().module.module_description;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "MyTasks", "TaskController");
            }

            return View(viewmodel);
        }

        public ActionResult MyOwnedTasks(SearchDTO _searchCriteria)
        {
            IList<TaskDM> viewmodel = null;
            try
            {
                int loggedin_user = Convert.ToInt32(Convert.ToString(this.Session["emp_syscode"]));

                TaskUserMapping taskUser = new TaskUserMapping();
                taskUser.employee_syscode = loggedin_user;
                taskUser.searchCriteria = _searchCriteria;

                var response = client.PostAsJsonAsync(cWebApiNames.APIGetMyOwnedTasks, taskUser).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    viewmodel = JsonConvert.DeserializeObject<IList<TaskDM>>(responseMsg);
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "MyTasks", "TaskController");
            }

            return View(viewmodel);
        }

        public ActionResult ViewTask(string id)
        {
            TaskViewModel viewmodel = null;
            try
            {
                string sreturn = string.Empty;
                int loggedin_user = Convert.ToInt32(Convert.ToString(this.Session["emp_syscode"]));
                int task_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));

                viewmodel = new TaskViewModel();
                viewmodel.task_syscode = task_syscode;
                viewmodel.logged_in_user = loggedin_user;
                viewmodel.group_syscode = ssGroupSyscode;
                string predcName = nameof(viewmodel.group_syscode);

                viewmodel.ddlData.Predicate[DBTableNameEnums.TaskPriorityMaster]["GetData"] = true;
                viewmodel.ddlData.Predicate[DBTableNameEnums.TaskStatusMaster]["GetData"] = true;
                viewmodel.ddlData.Predicate[DBTableNameEnums.vw_employee_master]["GetData"] = true;
                viewmodel.ddlData.Predicate[DBTableNameEnums.GroupMember]["GetData"] = true;
                viewmodel.ddlData.Predicate[DBTableNameEnums.GroupMember].Add(predcName, ssGroupSyscode);

                //int dept_syscode = Convert.ToInt32(Convert.ToString(this.Session["department_syscode"]));
                //string predName = nameof(vw_employee_master.department_syscode);
                //viewmodel.ddlData.Predicate[DBTableNameEnums.vw_employee_master].Add(predName, dept_syscode);

                if (task_syscode <= 0)
                {
                    throw new Exception("Invalid Task");
                }

                var response = client.PostAsJsonAsync(cWebApiNames.APIViewTaskByID, viewmodel).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    viewmodel = JsonConvert.DeserializeObject<TaskViewModel>(responseMsg);
                    if (viewmodel.opStatus)
                    {
                        viewmodel.SLPriority = new SelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.TaskPriorityMaster), "Value", "Text", 0);

                        //List<string> disabledValues = new List<string>();

                        //if (viewmodel.lstSubtasks.Count > 0)
                        //{
                        //    disabledValues.Add(((int)Enum_Master.StatusEnum.Complete).ToString());
                        //    disabledValues.Add(((int)Enum_Master.StatusEnum.Discard).ToString());
                        //}
                        //if (viewmodel.task_status_syscode == (int)Enum_Master.StatusEnum.Complete)
                        //{
                        //    disabledValues.Add(((int)Enum_Master.StatusEnum.Discard).ToString());
                        //    disabledValues.Add(((int)Enum_Master.StatusEnum.Open).ToString());
                        //    disabledValues.Add(((int)Enum_Master.StatusEnum.InProgress).ToString());
                        //}
                        //else if (viewmodel.task_status_syscode == (int)Enum_Master.StatusEnum.Open)
                        //{
                        //    disabledValues.Add(((int)Enum_Master.StatusEnum.Acknowledge).ToString());
                        //    disabledValues.Add(((int)Enum_Master.StatusEnum.InProgress).ToString());
                        //    disabledValues.Add(((int)Enum_Master.StatusEnum.Complete).ToString());
                        //}


                        viewmodel.SLStatus = new SelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.TaskStatusMaster), "Value", "Text", 0, viewmodel.ddlData.DisabledValues[DBTableNameEnums.TaskStatusMaster]);

                        var trailList = viewmodel.lstTrail
                                                  .Where(t => t.activity_syscode == (int)Enum_Master.ActivityEnum.Added_File || t.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments)
                                                  .Select(x => new TrailFileCommentVM
                                                  {
                                                      TrailDesc = x.trail_description,
                                                      TrailFiles = x.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments ? "" : x.trail_comments
                                                      ,
                                                      isDuplicate = x.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments && viewmodel.lstTrail.Where(c => c.activity_syscode == (int)Enum_Master.ActivityEnum.Added_File && c.trail_group_id == x.trail_group_id).Count() > 0 ? true : false
                                                     ,
                                                      TrailComment = x.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments ? x.trail_comments : viewmodel.lstTrail.Where(c => c.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments && c.trail_group_id == x.trail_group_id).FirstOrDefault()?.trail_comments
                                                  })
                                                  .ToList();

                        trailList = trailList.Except(trailList.Where(t => t.isDuplicate)).ToList();


                        viewmodel.lstTrailCommentsVM = trailList;




                        //viewmodel.lstTrailTemp = viewmodel.lstTrail
                        //                          .Where(t => t.activity_syscode == (int)Enum_Master.ActivityEnum.Added_File)
                        //                          .ToList();
                        //viewmodel.lstTrailTemp.ForEach(t => { t.trail_comments = viewmodel.lstTrail.Where(c => c.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments && c.trail_group_id == t.trail_group_id).FirstOrDefault().trail_comments; });


                    }
                }
                else
                {
                    throw new Exception(responseMsg);
                }
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.ErrorMessage = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");
            }
            return View(viewmodel);
        }

        public ActionResult CreateTask()
        {
            TaskViewModel taskViewModel = null;
            try
            {
                string sreturn = string.Empty;
                string groupPredName = nameof(taskViewModel.group_syscode);

                taskViewModel = new TaskViewModel();

                taskViewModel.ddlData.Predicate[DBTableNameEnums.TaskPriorityMaster]["GetData"] = true;
                taskViewModel.ddlData.Predicate[DBTableNameEnums.TaskStatusMaster]["GetData"] = true;
                taskViewModel.ddlData.Predicate[DBTableNameEnums.vw_employee_master]["GetData"] = true;
                taskViewModel.ddlData.Predicate[DBTableNameEnums.CategoryMaster]["GetData"] = true;
                taskViewModel.ddlData.Predicate[DBTableNameEnums.CategoryMaster].Add(groupPredName, ssGroupSyscode);
                taskViewModel.ddlData.Predicate[DBTableNameEnums.GroupMember]["GetData"] = true;
                taskViewModel.ddlData.Predicate[DBTableNameEnums.GroupMember].Add(groupPredName, ssGroupSyscode);

                DDLDTO ddldata = taskViewModel.ddlData;

                var response = client.PostAsJsonAsync(cWebApiNames.APIGetDDLData, ddldata).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    taskViewModel.ddlData = JsonConvert.DeserializeObject<DDLDTO>(responseMsg);
                    if (taskViewModel.ddlData.opStatus)
                    {
                        taskViewModel.SLPriority = new SelectList(taskViewModel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.TaskPriorityMaster), "Value", "Text", 0);
                        taskViewModel.SLCategory = new SelectList(taskViewModel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.CategoryMaster), "Value", "Text", 0);
                        taskViewModel.SLUsers = new SelectList(taskViewModel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.vw_employee_master), "Value", "Text", 0);
                        taskViewModel.MSLEmployees = new MultiSelectList(taskViewModel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.vw_employee_master), "Value", "Text", taskViewModel.arrUserSyscodes);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "CreateTask", "TaskController");
            }


            return View(taskViewModel);
        }

        public ActionResult CreateSubTask(string id)
        {
            TaskViewModel viewmodel = null;
            try
            {
                string sreturn = string.Empty;
                int task_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));
                if (task_syscode <= 0)
                {
                    throw new Exception("Invalid Task");
                }
                string groupPredName = nameof(CategoryMaster.group_syscode);

                viewmodel = new TaskViewModel();
                viewmodel.task_syscode = task_syscode;

                viewmodel.ddlData.Predicate[DBTableNameEnums.TaskPriorityMaster]["GetData"] = true;
                viewmodel.ddlData.Predicate[DBTableNameEnums.vw_employee_master]["GetData"] = true;
                viewmodel.ddlData.Predicate[DBTableNameEnums.CategoryMaster]["GetData"] = true;
                viewmodel.ddlData.Predicate[DBTableNameEnums.CategoryMaster].Add(groupPredName, ssGroupSyscode);
                viewmodel.ddlData.Predicate[DBTableNameEnums.GroupMember]["GetData"] = true;
                viewmodel.ddlData.Predicate[DBTableNameEnums.GroupMember].Add(groupPredName, ssGroupSyscode);

                var response = client.PostAsJsonAsync(cWebApiNames.APIViewTaskByID, viewmodel).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    viewmodel = JsonConvert.DeserializeObject<TaskViewModel>(responseMsg);
                    if (viewmodel.opStatus)
                    {
                        viewmodel.SLPriority = new SelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.TaskPriorityMaster), "Value", "Text", 0);
                        viewmodel.SLCategory = new SelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.CategoryMaster), "Value", "Text", 0);
                        if (viewmodel.module_syscode > 0 && viewmodel.level_syscode > 0)
                        {
                            viewmodel.isWorkflowTask = true;
                            viewmodel.SLUsers = new SelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.GroupMember), "Value", "Text", 0);
                            viewmodel.MSLEmployees = new MultiSelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.GroupMember), "Value", "Text", viewmodel.arrUserSyscodes);
                        }
                        else
                        {
                            viewmodel.SLUsers = new SelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.vw_employee_master), "Value", "Text", 0);
                            viewmodel.MSLEmployees = new MultiSelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.vw_employee_master), "Value", "Text", viewmodel.arrUserSyscodes);
                        }
                    }
                    else
                        throw new Exception(viewmodel.opMsg);
                }
                else
                {
                    throw new Exception(responseMsg);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "CreateTask", "TaskController");
            }


            return View(viewmodel);
        }


        public ActionResult SetWeightage(string id)
        {
            TaskViewModel viewmodel = null;
            try
            {
                string sreturn = string.Empty;
                int task_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));
                if (task_syscode <= 0)
                {
                    throw new Exception("Invalid Task");
                }

                viewmodel = new TaskViewModel();
                viewmodel.task_syscode = task_syscode;

                var response = client.PostAsJsonAsync(cWebApiNames.APIGetTaskSubTasks, viewmodel).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    viewmodel = JsonConvert.DeserializeObject<TaskViewModel>(responseMsg);
                    if (viewmodel.opStatus)
                    {

                    }
                }
                else
                {
                    throw new Exception(responseMsg);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "SetWeightage", "TaskController");
            }


            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult SaveWeightage(TaskViewModel viewmodel)
        {
            try
            {
                string sreturn = string.Empty;
                //int emp_syscode = int.Parse(Session["emp_syscode"].ToString());

                viewmodel.logged_in_user = ssLoggedInEmpSyscode;
                viewmodel.created_by = ssLoggedInEmpSyscode;
                viewmodel.strCreatedBy = ssLoggedInEmpName;//this.Session["emp_name"].ToString();

                var response = client.PostAsJsonAsync(cWebApiNames.APISaveWeightage, viewmodel).Result;

                if (response.IsSuccessStatusCode)
                {
                    var Response = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<TaskViewModel>(Response);
                    if (viewmodel.opStatus)
                    {
                        if (!string.IsNullOrEmpty(viewmodel.opMsg))
                        {
                            ViewBag.SuccessMessage = viewmodel.opMsg;
                            TempData["SuccessMessage"] = viewmodel.opMsg;
                        }
                        return RedirectToAction("SetWeightage", new { id = ComLibCommon.Base64Encode(viewmodel.task_syscode + "") });
                    }
                    else
                    {
                        throw new Exception(viewmodel.opMsg, viewmodel.opInnerException);
                    }
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "SaveWeightage", "TaskController");
            }
            return View("SetWeightage", viewmodel);
        }

        [HttpPost]
        public ActionResult SaveSubTask(TaskViewModel viewmodel)
        {
            try
            {
                string sreturn = string.Empty;
                var response = new HttpResponseMessage();

                if (string.IsNullOrEmpty(viewmodel.task_subject))
                {
                    throw new Exception("Please enter Task Subject");
                }

                viewmodel.logged_in_user = ssLoggedInEmpSyscode;
                viewmodel.created_by = ssLoggedInEmpSyscode;
                viewmodel.strCreatedBy = ssLoggedInEmpName;
                viewmodel.parent_task_syscode = viewmodel.task_syscode;
                viewmodel.isSubTask = true;

                var formCont = Request.returnMultipartFormData(viewmodel);
                response = client.PostAsync(cWebApiNames.APISaveTask, formCont).Result;


                if (response.IsSuccessStatusCode)
                {
                    var Response = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<TaskViewModel>(Response);
                    if (viewmodel.opStatus)
                    {
                        ViewBag.SuccessMessage = viewmodel.opMsg;
                        return RedirectToAction("ViewTask", new { id = ComLibCommon.Base64Encode(viewmodel.task_syscode + "") });
                    }
                    else
                    {
                        throw new Exception(viewmodel.opMsg, viewmodel.opInnerException);
                    }
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");
            }
            return View("CreateSubTask", viewmodel);
        }

        [HttpPost]
        public ActionResult SaveStandaloneTask(TaskViewModel viewmodel)
        {
            try
            {
                var response = new HttpResponseMessage();

                if (string.IsNullOrEmpty(viewmodel.task_subject))
                {
                    throw new Exception("Please enter Task Subject");
                }

                viewmodel.logged_in_user = ssLoggedInEmpSyscode;
                viewmodel.created_by = ssLoggedInEmpSyscode;
                viewmodel.strCreatedBy = ssLoggedInEmpName;
                viewmodel.logged_in_user_name = ssLoggedInEmpName;
                viewmodel.level_syscode = null;
              

                var formCont = Request.returnMultipartFormData(viewmodel);
                response = client.PostAsync(cWebApiNames.APISaveTask, formCont).Result;

                if (response.IsSuccessStatusCode)
                {
                    var Response = response.Content.ReadAsStringAsync().Result;
                    viewmodel = JsonConvert.DeserializeObject<TaskViewModel>(Response);
                    if (viewmodel.opStatus)
                    {
                        ViewBag.SuccessMessage = viewmodel.opMsg;

                        return RedirectToAction("ViewTask", new { id = ComLibCommon.Base64Encode(viewmodel.task_syscode + "") });
                    }
                    else
                    {
                        throw new Exception(viewmodel.opMsg, viewmodel.opInnerException);
                    }
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "SaveStandaloneTask", "TaskController");
            }
            return View("CreateTask", viewmodel);
        }


        [HttpPost]
        public ActionResult SaveTrail(TaskViewModel viewmodel)
        {
            try
            {
                if (viewmodel.task_syscode <= 0)
                {
                    throw new Exception("Invalid Task");
                }

                viewmodel.group_syscode = ssGroupSyscode;
                viewmodel.created_by = ssLoggedInEmpSyscode;
                viewmodel.logged_in_user = ssLoggedInEmpSyscode;
                viewmodel.logged_in_user_name = ssLoggedInEmpName;
                viewmodel.ddlData.Predicate[DBTableNameEnums.TaskPriorityMaster]["GetData"] = true;
                viewmodel.ddlData.Predicate[DBTableNameEnums.TaskStatusMaster]["GetData"] = true;
                viewmodel.ddlData.Predicate[DBTableNameEnums.vw_employee_master]["GetData"] = true;

                var formCont = Request.returnMultipartFormData(viewmodel);

                var response = client.PostAsync(cWebApiNames.APISaveTaskTrail, formCont).Result;

                if (response.IsSuccessStatusCode)
                {
                    var clientResponse = response.Content.ReadAsStringAsync().Result;
                    var responseMsg = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        OperationDetailsDTO od = JsonConvert.DeserializeObject<OperationDetailsDTO>(responseMsg);

                        ///ToDo: Find Better way of deserializing actual viewmodel, in case of the exception.

                        if (od.opStatus)
                        {
                            /// To be Redirected to ViewTask from here
                            viewmodel = JsonConvert.DeserializeObject<TaskViewModel>(responseMsg);

                            viewmodel.SLPriority = new SelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.TaskPriorityMaster), "Value", "Text", 0);
                            viewmodel.SLCategory = new SelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.CategoryMaster), "Value", "Text", 0);
                            viewmodel.SLUsers = new SelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.vw_employee_master), "Value", "Text", 0);
                            viewmodel.SLStatus = new SelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.TaskStatusMaster), "Value", "Text", 0, viewmodel.ddlData.DisabledValues[DBTableNameEnums.TaskStatusMaster]);

                            var trailList = viewmodel.lstTrail
                                                      .Where(t => t.activity_syscode == (int)Enum_Master.ActivityEnum.Added_File || t.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments)
                                                      .Select(x => new TrailFileCommentVM
                                                      {
                                                          TrailDesc = x.trail_description,
                                                          TrailFiles = x.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments ? "" : x.trail_comments
                                                          ,
                                                          isDuplicate = x.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments && viewmodel.lstTrail.Where(c => c.activity_syscode == (int)Enum_Master.ActivityEnum.Added_File && c.trail_group_id == x.trail_group_id).Count() > 0 ? true : false
                                                         ,
                                                          TrailComment = x.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments ? x.trail_comments : viewmodel.lstTrail.Where(c => c.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Comments && c.trail_group_id == x.trail_group_id).FirstOrDefault()?.trail_comments
                                                      })
                                                      .ToList();

                            trailList = trailList.Except(trailList.Where(t => t.isDuplicate)).ToList();
                            viewmodel.lstTrailCommentsVM = trailList;
                            ModelState.Clear();
                            if (viewmodel.opInnerException != null)
                            {
                                throw new Exception(viewmodel.opMsg);
                            }
                        }
                        else
                        {
                            throw new Exception(od.opMsg);
                        }
                    }
                    else
                    {
                        throw new Exception(responseMsg);
                    }

                }

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "SaveTrail", "TaskController");
            }
            return View("ViewTask", viewmodel);
        }


        public JsonResult SaveUserActivity(string jsonData)        // Start/Stop Task
        {
            string mIsSuccess = string.Empty;
            try
            {
                if (jsonData == null)
                {
                    throw new Exception("Invalid Json Data.");
                }

                var pageDetails = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);
                string encTasksyscode = pageDetails["task_syscode"].ToString();
                string action = pageDetails["action"].ToString();
                int task_syscode = String.IsNullOrEmpty(encTasksyscode) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(encTasksyscode));

                if (task_syscode == 0 || !(action.Equals("start") || action.Equals("stop")))
                {
                    throw new Exception("Invalid Task.");
                }

                TaskUserRecordDM taskUserRecordDM = new TaskUserRecordDM();
                taskUserRecordDM.task_syscode = task_syscode;
                taskUserRecordDM.UserAction = action;
                taskUserRecordDM.employee_syscode = ssLoggedInEmpSyscode;
                taskUserRecordDM.logged_in_user = ssLoggedInEmpSyscode;

                HttpResponseMessage response = null;
                if (action.Equals("start"))
                {
                    taskUserRecordDM.start_time = DateTime.Now;
                    response = client.PostAsJsonAsync(cWebApiNames.APISaveTaskStart, taskUserRecordDM).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var Response = response.Content.ReadAsStringAsync().Result;
                        taskUserRecordDM = JsonConvert.DeserializeObject<TaskUserRecordDM>(Response);
                        if (taskUserRecordDM.opStatus)
                        {
                            mIsSuccess = "Y" + taskUserRecordDM.start_time.Value.ToString("ddd, dd MMM yyyy HH:mm:ss");
                        }
                        else
                        {
                            throw new Exception(taskUserRecordDM.opMsg, taskUserRecordDM.opInnerException);
                        }
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
                else if (action.Equals("stop"))
                {
                    taskUserRecordDM.stop_time = DateTime.Now;
                    response = client.PostAsJsonAsync(cWebApiNames.APISaveTaskStop, taskUserRecordDM).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var Response = response.Content.ReadAsStringAsync().Result;
                        taskUserRecordDM = JsonConvert.DeserializeObject<TaskUserRecordDM>(Response);
                        if (taskUserRecordDM.opStatus)
                        {
                            mIsSuccess = "Y" + taskUserRecordDM.stop_time.Value.ToString("ddd, dd MMM yyyy HH:mm:ss");
                        }
                        else
                        {
                            throw new Exception(taskUserRecordDM.opMsg, taskUserRecordDM.opInnerException);
                        }
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }


                return Json(mIsSuccess, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                mIsSuccess = "Error:" + ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "SaveUserActivity", "TaskController");
                return Json(mIsSuccess, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult TaskReport(SearchDTO _searchCriteria)
        {
            TaskViewModel viewmodel = null;
            try
            {
                _searchCriteria = FillSearchDTO(_searchCriteria);

                viewmodel = new TaskViewModel();
                _searchCriteria.group_syscode = ssGroupSyscode;
                viewmodel.searchDTO = _searchCriteria;

                TaskUserMapping taskUser = new TaskUserMapping();
                taskUser.employee_syscode = ssLoggedInEmpSyscode;
                taskUser.searchCriteria = _searchCriteria;

                var response = client.PostAsJsonAsync(cWebApiNames.APIGetTaskReport, taskUser).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    viewmodel.lstTaskDM = JsonConvert.DeserializeObject<List<TaskDM>>(responseMsg);
                    viewmodel.searchDTO = _searchCriteria;
                    //if (viewmodel.lstTaskDM != null && viewmodel.lstTaskDM.Count > 0)
                    //{///ToDo: Ask Aartiji logic behind this //We agreed to remove it.
                    //    viewmodel.module.module_description = viewmodel.lstTaskDM.First()?.module?.module_description; 
                    //}
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "MyTasks", "TaskController");
            }

            return View(viewmodel);
        }

        
      
        //public ActionResult ViewSubTasks(string id)
        //{
        //    IList<TaskDM> viewmodel = null;
        //    try
        //    {
        //        string sreturn = string.Empty;
        //        if (this.Session["emp_syscode"] != null)
        //        {
        //            int loggedin_user = Convert.ToInt32(Convert.ToString(this.Session["emp_syscode"]));
        //            int task_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));
        //            if (task_syscode <= 0)
        //            {
        //                throw new Exception("Invalid Task");
        //            }

        //            TaskUserMapping taskUser = new TaskUserMapping();
        //            taskUser.employee_syscode = loggedin_user;
        //            taskUser.task_syscode = task_syscode;

        //            var response = client.PostAsJsonAsync(cWebApiNames.APIGetSubTasks, taskUser).Result;
        //            var responseMsg = response.Content.ReadAsStringAsync().Result;
        //            if (response.IsSuccessStatusCode)
        //            {
        //                viewmodel = JsonConvert.DeserializeObject<IList<TaskDM>>(responseMsg);
        //            }
        //            else
        //            {
        //                throw new Exception(response.ReasonPhrase);
        //            }
        //        }
        //        else
        //        {
        //             return RedirectToAction("Logout", "Login", new { Area = ""});
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = ex.Message;
        //        ModelState.AddModelError("KeyException", ex.Message);
        //        Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "MyTasks", "TaskController");
        //    }

        //    return View(viewmodel);
        //}

    }
}