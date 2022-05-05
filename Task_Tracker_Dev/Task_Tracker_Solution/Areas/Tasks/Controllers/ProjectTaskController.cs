using Common_Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Solution.Areas.Common.Controllers;
using Task_Tracker_Solution.Areas.Tasks.Models;
using Task_Tracker_Solution.Utility;

namespace Task_Tracker_Solution.Areas.Tasks
{
    public class ProjectTaskController : TaskTrackerBaseController
    {
        // GET: Tasks/ProjectTask
        public ActionResult CreateProjectTask(SearchDTO _searchCriteria)
        {
            TaskViewModel viewmodel = null;
            try
            {
                if (_searchCriteria == null)
                {
                    throw new Exception("Search filter can not be Null.");
                }

                _searchCriteria.actionName = "CreateProjectTask";
                _searchCriteria.employee_syscode = ssLoggedInEmpSyscode;

                _searchCriteria = FillSearchDTO(_searchCriteria);

                viewmodel = new TaskViewModel();
                viewmodel.searchDTO = _searchCriteria;

                if (_searchCriteria.module_syscode > 0)
                {
                    viewmodel.module_syscode = _searchCriteria.module_syscode;
                    viewmodel.task_syscode = _searchCriteria.task_syscode;
                    viewmodel.logged_in_user = ssLoggedInEmpSyscode;
                    viewmodel.project.project_syscode = _searchCriteria.project_syscode;

                    string groupPredName = nameof(CategoryMaster.group_syscode);

                    viewmodel.ddlData.Predicate[DBTableNameEnums.TaskPriorityMaster]["GetData"] = true;
                    viewmodel.ddlData.Predicate[DBTableNameEnums.GroupMember]["GetData"] = true;
                    viewmodel.ddlData.Predicate[DBTableNameEnums.GroupMember].Add(groupPredName, ssGroupSyscode);
                    
                    var response = client.PostAsJsonAsync(cWebApiNames.APIGetProjectTaskInfo, viewmodel).Result;
                    var responseMsg = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        viewmodel = JsonConvert.DeserializeObject<TaskViewModel>(responseMsg);
                        viewmodel.searchDTO = _searchCriteria;
                        if (viewmodel.ddlData.opStatus)
                        {
                            viewmodel.SLPriority = new SelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.TaskPriorityMaster), "Value", "Text", 0);
                            viewmodel.SLUsers = new SelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.GroupMember), "Value", "Text", 0);
                            viewmodel.MSLEmployees = new MultiSelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.GroupMember), "Value", "Text", viewmodel.arrUserSyscodes);
                        }
                        else
                            throw new Exception(viewmodel.opMsg);
                    }
                    else
                        throw new Exception(response.ReasonPhrase);

                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "MyTasks", "TaskController");
                viewmodel.searchDTO = _searchCriteria;
            }

            viewmodel.searchDTO.enableSearchTextBox = false; //Disable search textbox that appears on the listing pages.
            viewmodel.isProjectTaskPage = true;
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult SaveProjectTask(TaskViewModel viewmodel)
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
                
                var formCont = Request.returnMultipartFormData(viewmodel);
                response = client.PostAsync(cWebApiNames.APISaveProjectTask, formCont).Result;

                if (response.IsSuccessStatusCode)
                {
                    var Response = response.Content.ReadAsStringAsync().Result;
                    var task = JsonConvert.DeserializeObject<JMTask>(Response);
                    if (task.opStatus)
                    {
                        ViewBag.SuccessMessage = task.opMsg;
                        SearchDTO _searchDTO = new SearchDTO()
                        {
                            project_syscode = viewmodel.searchDTO.project_syscode,
                            category_syscode = viewmodel.searchDTO.category_syscode,
                            module_syscode = viewmodel.searchDTO.module_syscode,
                            task_syscode = task.task_syscode
                        };
                        TempData["ObjSearchDTO"] = _searchDTO;
                        return RedirectToAction("MyCreatedTasks", "Task", new { Area = "Tasks" });
                    }
                    else
                        throw new Exception(viewmodel.opMsg, viewmodel.opInnerException);
                }
                else
                    throw new Exception(response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "MyTasks", "TaskController");

                viewmodel = new TaskViewModel();
                viewmodel.searchDTO.actionName = "CreateProjectTask";
                viewmodel.searchDTO.employee_syscode = ssLoggedInEmpSyscode;
                viewmodel.searchDTO = FillSearchDTO(viewmodel.searchDTO);
                viewmodel.searchDTO.enableSearchTextBox = false; 
                viewmodel.isProjectTaskPage = true;
            }
            return View("CreateProjectTask", viewmodel);
        }
    }
}