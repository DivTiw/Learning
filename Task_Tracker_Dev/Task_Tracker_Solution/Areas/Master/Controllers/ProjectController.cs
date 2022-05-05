using Common_Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Solution.Areas.Common.Controllers;
using Task_Tracker_Solution.Areas.Master.Models;
using Task_Tracker_Solution.Utility;

namespace Task_Tracker_Solution.Areas.Master.Controllers
{
    public class ProjectController : TaskTrackerBaseController
    {
        // GET: Master/Project

        public ProjectController()
        {
        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult CreateProject()
        {
            ProjectViewModel projmodel = null;
            try
            {
                ViewBag.Title = "Create Project";
                string sreturn = string.Empty;

                projmodel = new ProjectViewModel();
                projmodel.logged_in_user = ssLoggedInEmpSyscode;
                string predcName = nameof(projmodel.group_syscode);

                projmodel.ddlData.Predicate[DBTableNameEnums.GroupMember]["GetData"] = true;
                projmodel.ddlData.Predicate[DBTableNameEnums.GroupMember].Add(predcName, ssGroupSyscode);

                DDLDTO ddldata = projmodel.ddlData;

                projmodel.group_syscode = ssGroupSyscode;
                var response = client.PostAsJsonAsync(cWebApiNames.APIGetDDLData, ddldata).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    projmodel.ddlData = JsonConvert.DeserializeObject<DDLDTO>(responseMsg);
                    if (projmodel.ddlData.opStatus)
                    {
                        //projmodel.SLWorkFlow = projmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.WorkflowMaster);
                        //projmodel.WorkflowListJson = JsonConvert.SerializeObject(projmodel.SLWorkFlow);
                        //projmodel.ModulesListJson = JsonConvert.SerializeObject(projmodel.lstModules);
                        projmodel.SLGroupMembers = projmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.GroupMember);
                        projmodel.MemberListJson = JsonConvert.SerializeObject(projmodel.SLGroupMembers);
                        projmodel.PageHasWriteAccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");
            }

            return View(projmodel);
        }

        public ActionResult EditProject(string id = null)
        {
            ProjectViewModel projmodel = new ProjectViewModel();
            try
            {
                ViewBag.Title = "Edit Project";
                string sreturn = string.Empty;

                int proj_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));
                string predcName = nameof(projmodel.group_syscode);

                if (proj_syscode > 0) //Edit Project
                {
                    projmodel = new ProjectViewModel();
                    projmodel.logged_in_user = ssLoggedInEmpSyscode;
                    projmodel.project_syscode = proj_syscode;

                    projmodel.ddlData.Predicate[DBTableNameEnums.GroupMember]["GetData"] = true;
                    projmodel.ddlData.Predicate[DBTableNameEnums.GroupMember].Add(predcName, ssGroupSyscode);

                    var response = client.PostAsJsonAsync(cWebApiNames.APIGetProjectByID, projmodel).Result;
                    var responseMsg = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        projmodel = JsonConvert.DeserializeObject<ProjectViewModel>(responseMsg);
                        if (projmodel.opStatus)
                        {
                            //projmodel.ModulesListJson = JsonConvert.SerializeObject(projmodel.lstModules);                                
                            //projmodel.SLWorkFlow = projmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.WorkflowMaster);//[DBTableNameEnums.WorkflowMaster];   
                            //projmodel.SLWorkFlow.Insert(0, new SelectItemDTO() { Text = "Select Workflow", Value = "0" });
                            //projmodel.WorkflowListJson = JsonConvert.SerializeObject(projmodel.SLWorkFlow);
                            projmodel.SLGroupMembers = projmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.GroupMember);
                            projmodel.MemberListJson = JsonConvert.SerializeObject(projmodel.SLGroupMembers);
                        }
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");

            }

            return View("CreateProject", projmodel);
        }

        public ActionResult MyProjects(SearchDTO _searchCriteria)
        {
            ProjectViewModel projmodel = null;
            try
            {
                string sreturn = string.Empty;
                
                _searchCriteria = FillSearchDTO(_searchCriteria);              
                _searchCriteria.group_syscode = ssGroupSyscode;
                _searchCriteria.enableSearchTextBox = false;
                _searchCriteria.enableStatusDD = false;
                _searchCriteria.enableTaskDD = false;
                _searchCriteria.enableCategoryDD = false;
                _searchCriteria.enableModuleDD = false;

                projmodel = new ProjectViewModel();
                projmodel.logged_in_user = ssLoggedInEmpSyscode;
                projmodel.group_syscode = ssGroupSyscode;
                projmodel.project_syscode = _searchCriteria.project_syscode;

                var response = client.PostAsJsonAsync(cWebApiNames.APIGetAllProjectList, projmodel).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    projmodel.lstProjDM = JsonConvert.DeserializeObject<IList<ProjectDM>>(responseMsg);
                    projmodel.searchDTO = _searchCriteria;
                }
                else
                {
                    throw new Exception(responseMsg);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");
            }

            return View(projmodel);

        }

        [HttpPost]
        public async Task<ActionResult> Submit(ProjectViewModel projmodel, string action)
        {
            try
            {
                string sreturn = string.Empty;

                var response = new HttpResponseMessage();

                string predcName = nameof(projmodel.group_syscode);

                projmodel.created_by = ssLoggedInEmpSyscode;
                projmodel.group_syscode = ssGroupSyscode;

                projmodel.ddlData.Predicate[DBTableNameEnums.GroupMember]["GetData"] = true;
                projmodel.ddlData.Predicate[DBTableNameEnums.GroupMember].Add(predcName, ssGroupSyscode);


                if (!string.IsNullOrEmpty(projmodel.MemberListJson))
                {
                    projmodel.SLGroupMembers = JsonConvert.DeserializeObject<List<SelectItemDTO>>(projmodel.MemberListJson);
                }
                if (projmodel.group_syscode <= 0)
                {
                    throw new Exception("Please select Group.");
                }
                //if (!string.IsNullOrEmpty(projmodel.ModulesListJson))
                //{
                //    projmodel.lstModules = JsonConvert.DeserializeObject<IList<ModuleMaster>>(projmodel.ModulesListJson);
                //    //projmodel.SLWorkFlow = JsonConvert.DeserializeObject<List<SelectItemDTO>>(projmodel.WorkflowListJson);
                //}
                //if (!string.IsNullOrEmpty(projmodel.WorkflowListJson))
                //{
                //    projmodel.SLWorkFlow = JsonConvert.DeserializeObject<List<SelectItemDTO>>(projmodel.WorkflowListJson);
                //    projmodel.SLWorkFlow.Insert(0, new SelectItemDTO() { Text = "Select Workflow", Value = "0" });
                //}
                #region "Save Project"

                if (action == "save_project" || action == "Submit")
                {
                    if (string.IsNullOrEmpty(projmodel.project_name))
                    {
                        throw new Exception("Please enter Project Name");
                    }

                    if (ComLibCommon.CheckDuplicateUser(projmodel.arrReadUserSyscodes, projmodel.arrWriteUserSyscodes))
                    {
                        throw new Exception("Read Access User cannot be added as Write Access User.");
                    }

                    if (projmodel.project_syscode > 0)
                    {
                        ViewBag.Title = "Edit Project";
                        projmodel.modified_by = ssLoggedInEmpSyscode;
                        response = await client.PutAsJsonAsync(Utility.cWebApiNames.APIUpdateProject, projmodel);
                    }
                    else
                    {
                        ViewBag.Title = "Create Project";
                        projmodel.lstModules = null; //During first time project creation, list modules is always null.
                        response = await client.PostAsJsonAsync(Utility.cWebApiNames.APIPostProject, projmodel);
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var Response = response.Content.ReadAsStringAsync().Result;
                        //ProjectViewModel pvm = JsonConvert.DeserializeObject<ProjectViewModel>(Response);
                        OperationDetailsDTO od = JsonConvert.DeserializeObject<OperationDetailsDTO>(Response);

                        if (od.opStatus)
                        {
                            //ProjectViewModel pvm = JsonConvert.DeserializeObject<ProjectViewModel>(Response);
                            ModelState.Clear();
                            //projmodel.project_syscode = projmodel.project_syscode;
                            projmodel.opMsg = od.opMsg;

                            //projmodel.SLGroupMembers = pvm.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.GroupMember);
                            ViewBag.SuccessMessage = od.opMsg;

                            //ViewBag.Title = "Edit Project";
                            return RedirectToAction("MyProjects");
                            //return RedirectToAction("EditProject", new { id = ComLibCommon.Base64Encode(projmodel.project_syscode + "")});//+"")
                        }
                        else
                        {
                            ViewBag.ErrorMessage = od.opMsg;
                            throw new Exception(od.opMsg, od.opInnerException);
                        }
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
                return View("CreateProject", projmodel);
                #endregion
                #region "Save Project Modules"
                //else
                //{
                //    if (projmodel.lstModules.Count > 0)
                //    {
                //        if (projmodel.lstModules.Any(m => m.workflow_syscode == 0 || string.IsNullOrEmpty(m.module_name)))
                //        {
                //            throw new Exception("Please check if you have entered the module name and selected a workflow.");
                //        }
                //        if (projmodel.lstModules.Any(x=> projmodel.lstModules.Count(y=> y.module_name == x.module_name) > 1)) //Join(projmodel.lstModules, mod=> mod.module_name, modCpy=> modCpy.module_name, (mod, modCpy)=> mod.module_name).Any()
                //        {
                //            throw new Exception("Please enter unique module names in the list.");
                //        }
                //        response = await client.PostAsJsonAsync(Utility.cWebApiNames.APIAddUpdateProjectModules, projmodel.lstModules);
                //        if (response.IsSuccessStatusCode)
                //        {
                //            var Response = response.Content.ReadAsStringAsync().Result;
                //            ProjectMaster pm = JsonConvert.DeserializeObject<ProjectMaster>(Response);
                //            if (pm.opStatus)
                //            {
                //                projmodel.lstModules = pm.lstModules;
                //                projmodel.ModulesListJson = JsonConvert.SerializeObject(projmodel.lstModules);
                //                ViewBag.SuccessMessage = pm.opMsg;//"Record saved successfully.";
                //                //TempData.Add("SuccessMessage", "Record saved successfully!");
                //                //TempData.Keep("SuccessMessage");

                //                ViewBag.Title = "Edit Project";
                //                return View("CreateProject", projmodel);//return RedirectToAction("EditProject", new { id = ComLibCommon.Base64Encode(projmodel.project_syscode + "") });
                //            }
                //            else
                //            {
                //                ViewBag.ErrorMessage = "Error occurred while saving the record.";
                //                throw new Exception(pm.opMsg);
                //            }
                //        }
                //        else
                //        {
                //            throw new Exception(response.ReasonPhrase);
                //        }
                //    }
                //    else
                //    {
                //        ViewBag.SuccessMessage = "Please add aleast one module for saving.";
                //        return View("CreateProject", projmodel);
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                //TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");
                if (projmodel.project_syscode > 0)
                {
                    ViewBag.Title = "Edit Project";
                }
                else
                {
                    ViewBag.Title = "Create Project";
                }
                return View("CreateProject", projmodel);
            }
        }

        #region ProjectDetails

        public ActionResult ProjectDetails(string id, string projname)
        {
            ProjectViewModel projmodel = new ProjectViewModel();
            try
            {

                string sreturn = string.Empty;
                ViewBag.SuccessMessage = TempData["SuccessMessage"];

                int proj_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));
                string predcName = nameof(projmodel.group_syscode);

                if (proj_syscode > 0)
                {
                    projmodel = new ProjectViewModel();
                    projmodel.logged_in_user = ssLoggedInEmpSyscode;
                    projmodel.project_syscode = proj_syscode;
                    projmodel.project_name = projname;
                    projmodel.group_syscode = ssGroupSyscode;

                    DDLDTO ddldata = FillDD_Env_Parameter(projmodel.ddlData);
                    if (ddldata.opStatus)
                    {
                        projmodel.SLEnvironment = new SelectList(ddldata.Data.ExtractDDLDataForKey(DBTableNameEnums.EnvironmentMaster), "Value", "Text", 0);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");

            }
            return View(projmodel);
        }


        [HttpPost]
        public ActionResult GetProjectDetails(ProjectViewModel projmodel)
        {
            try
            {
                string sreturn = string.Empty;

                var response = new HttpResponseMessage();

                string predcName = nameof(projmodel.group_syscode);

                projmodel.created_by = ssLoggedInEmpSyscode;
                projmodel.group_syscode = ssGroupSyscode;

                if (projmodel.group_syscode <= 0)
                {
                    throw new Exception("Please select Group.");
                }

                if (projmodel.project_syscode <= 0)
                {
                    throw new Exception("Please select Project.");
                }
                if (projmodel.env_syscode <= 0)
                {
                    throw new Exception("Please select Environment.");
                }

                projmodel.ddlData.Predicate[DBTableNameEnums.EnvironmentMaster]["GetData"] = true;
                projmodel.ddlData.Predicate[DBTableNameEnums.EnvironmentMaster].Add(predcName, ssGroupSyscode);
                projmodel.ddlData.Predicate[DBTableNameEnums.ProjectParameterMaster]["GetData"] = true;
                projmodel.ddlData.Predicate[DBTableNameEnums.ProjectParameterMaster].Add(predcName, ssGroupSyscode);

                response = client.PostAsJsonAsync(cWebApiNames.APIGetProjectDetails, projmodel).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    projmodel = JsonConvert.DeserializeObject<ProjectViewModel>(responseMsg);
                    projmodel.lstProjDetailsJson = JsonConvert.SerializeObject(projmodel.lstProjDetails);
                    projmodel.SLEnvironment = new SelectList(projmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.EnvironmentMaster), "Value", "Text", projmodel.env_syscode);
                    projmodel.lstParameter = projmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.ProjectParameterMaster) ?? new List<SelectItemDTO>();

                }
                else
                {
                    throw new Exception(responseMsg);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                //TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "GetProjectDetails", "ProjectController");

            }
            return View("ProjectDetails", projmodel);
        }

        [HttpPost]
        public async Task<ActionResult> Submit_ProjectDetails(ProjectViewModel projmodel)
        {
            try
            {
                string sreturn = string.Empty;

                var response = new HttpResponseMessage();
                string project_name = projmodel.project_name;
                string predcName = nameof(projmodel.group_syscode);

                projmodel.created_by = ssLoggedInEmpSyscode;
                projmodel.group_syscode = ssGroupSyscode;

                if (projmodel.group_syscode <= 0)
                {
                    throw new Exception("Please select Group.");
                }

                if (projmodel.project_syscode <= 0)
                {
                    throw new Exception("Please select Project.");
                }
                if (projmodel.env_syscode <= 0)
                {
                    throw new Exception("Please select Environment.");
                }
                DDLDTO ddldata = FillDD_Env_Parameter(projmodel.ddlData);
                if (ddldata.opStatus)
                {
                    projmodel.SLEnvironment = new SelectList(ddldata.Data.ExtractDDLDataForKey(DBTableNameEnums.EnvironmentMaster), "Value", "Text", 0);
                    projmodel.lstParameter = ddldata.Data.ExtractDDLDataForKey(DBTableNameEnums.ProjectParameterMaster) ?? new List<SelectItemDTO>();
                }
                if (!string.IsNullOrEmpty(projmodel.lstProjDetailsJson))
                {
                    projmodel.lstProjDetails = JsonConvert.DeserializeObject<IList<ProjectDetails>>(projmodel.lstProjDetailsJson);
                    if (projmodel.lstProjDetails.Count > 0 && !projmodel.lstProjDetails.Any(x => x.project_syscode <= 0))
                    {
                        if (projmodel.lstProjDetails.Any(m => m.parameter_syscode <= 0))
                        {
                            throw new Exception("Please select Parameter.");
                        }

                        if (projmodel.lstProjDetails.Any(m => m.parameter_value == string.Empty))
                        {
                            throw new Exception("Parameter value cannot be empty.");
                        }

                        response = await client.PostAsJsonAsync(Utility.cWebApiNames.APIAddUpdateProjectDetails, projmodel);//lstWFLevels
                        if (response.IsSuccessStatusCode)
                        {
                            var Response = response.Content.ReadAsStringAsync().Result;
                            ProjectMaster pm = JsonConvert.DeserializeObject<ProjectMaster>(Response);
                            if (pm.opStatus)
                            {
                                projmodel.lstProjDetails = pm.lstProjDetails;
                                projmodel.lstProjDetailsJson = JsonConvert.SerializeObject(projmodel.lstProjDetails);
                                TempData.Add("SuccessMessage", "Record saved successfully!");
                                // return RedirectToAction("ProjectDetails", new { id = ComLibCommon.Base64Encode(projmodel.project_syscode + ""), projname = project_name });
                            }
                            else
                            {
                                throw new Exception(pm.opMsg);
                            }
                        }
                        else
                        {
                            throw new Exception(response.ReasonPhrase);
                        }
                    }
                    else
                        throw new Exception("List of Project Details cannot be empty!");

                }
            }
            catch (Exception ex)
            {                
                ViewBag.ErrorMessage = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "Submit_ProjectDetails", "ProjectController");

            }
            return View("ProjectDetails", projmodel);
        }

        private DDLDTO FillDD_Env_Parameter(DDLDTO ddldata)
        {

            string predcName = "group_syscode";

            ddldata.Predicate[DBTableNameEnums.EnvironmentMaster]["GetData"] = true;
            ddldata.Predicate[DBTableNameEnums.EnvironmentMaster].Add(predcName, ssGroupSyscode);
            ddldata.Predicate[DBTableNameEnums.ProjectParameterMaster]["GetData"] = true;
            ddldata.Predicate[DBTableNameEnums.ProjectParameterMaster].Add(predcName, ssGroupSyscode);


            var response = client.PostAsJsonAsync(cWebApiNames.APIGetDDLData, ddldata).Result;
            var responseMsg = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                ddldata = JsonConvert.DeserializeObject<DDLDTO>(responseMsg);
            }
            return ddldata;
        }



        public ActionResult ViewProjectDetails(string id, string projname)
        {
            ProjectViewModel projmodel = new ProjectViewModel();
            try
            {
                string sreturn = string.Empty;
                ViewBag.SuccessMessage = TempData["SuccessMessage"];

                int proj_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));
                string predcName = nameof(projmodel.group_syscode);

                if (proj_syscode > 0)
                {
                    projmodel = new ProjectViewModel();
                    projmodel.logged_in_user = ssLoggedInEmpSyscode;
                    projmodel.project_syscode = proj_syscode;
                    projmodel.project_name = projname;
                    projmodel.group_syscode = ssGroupSyscode;

                    DDLDTO ddldata = FillDD_Env_Parameter(projmodel.ddlData);
                    if (ddldata.opStatus)
                    {
                        projmodel.SLEnvironment = new SelectList(ddldata.Data.ExtractDDLDataForKey(DBTableNameEnums.EnvironmentMaster), "Value", "Text", 0);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");

            }
            return View(projmodel);
           
        }

        [HttpPost]
        public ActionResult Post_ViewProjectDetails(ProjectViewModel projmodel)
        {
            try
            {
                string sreturn = string.Empty;

                var response = new HttpResponseMessage();

                string predcName = nameof(projmodel.group_syscode);

                projmodel.created_by = ssLoggedInEmpSyscode;
                projmodel.group_syscode = ssGroupSyscode;

                if (projmodel.group_syscode <= 0)
                {
                    throw new Exception("Please select Group.");
                }

                if (projmodel.project_syscode <= 0)
                {
                    throw new Exception("Please select Project.");
                }
                if (projmodel.env_syscode <= 0)
                {
                    throw new Exception("Please select Environment.");
                }

                DDLDTO ddldata = FillDD_Env_Parameter(projmodel.ddlData);


                response = client.PostAsJsonAsync(cWebApiNames.APIGetProjectDetails, projmodel).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    projmodel = JsonConvert.DeserializeObject<ProjectViewModel>(responseMsg);
                    if (ddldata.opStatus)
                    {
                        projmodel.SLEnvironment = new SelectList(ddldata.Data.ExtractDDLDataForKey(DBTableNameEnums.EnvironmentMaster), "Value", "Text", projmodel.env_syscode);
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
                //TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "Post_ViewProjectDetails", "ProjectController");

            }
            return View("ViewProjectDetails", projmodel);
        }
        #endregion

        #region "Release Details"
        public ActionResult ViewReleaseDetails(string id, string projname)
        {
            ProjectViewModel projmodel = null;
            try
            {
                projmodel = new ProjectViewModel();
                string sreturn = string.Empty;
                ViewBag.SuccessMessage = TempData["SuccessMessage"];

                int proj_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));
                string predcName = nameof(projmodel.group_syscode);              

                if (proj_syscode > 0)
                {
                    projmodel = new ProjectViewModel();
                    projmodel.logged_in_user = ssLoggedInEmpSyscode;
                    projmodel.project_syscode = proj_syscode;
                    projmodel.project_name = projname;
                    projmodel.group_syscode = ssGroupSyscode;

                    DDLDTO ddldata = FillDD_Env_Parameter(projmodel.ddlData);
                    if (ddldata.opStatus)
                    {
                        projmodel.SLEnvironment = new SelectList(ddldata.Data.ExtractDDLDataForKey(DBTableNameEnums.EnvironmentMaster), "Value", "Text", 0);
                    }
                }
            }
            catch (Exception ex)
            {               
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "ViewReleaseDetails", "ProjectController");
                return Content("Error Occured: " + ex.Message );
            }
            //return View(projmodel);
            return PartialView("~/Areas/Tasks/Views/Task/_ReleaseDetails.cshtml", projmodel);

        }

        public ActionResult GetReleaseDetails(string id1, string id2)
        {
            ProjectViewModel projmodel = null;
            try
            {
                projmodel = new ProjectViewModel();
                var response = new HttpResponseMessage();

                int proj_syscode = String.IsNullOrEmpty(id1) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id1));
                int env_syscode = String.IsNullOrEmpty(id2) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id2));

                projmodel.created_by = ssLoggedInEmpSyscode;
                projmodel.group_syscode = ssGroupSyscode;
                projmodel.project_syscode = proj_syscode;
                projmodel.env_syscode = env_syscode;

                if (projmodel.group_syscode <= 0)
                {
                    throw new Exception("Please select Group.");
                }

                if (projmodel.project_syscode <= 0)
                {
                    throw new Exception("Please select Project.");
                }
                if (projmodel.env_syscode <= 0)
                {
                    throw new Exception("Please select Environment.");
                }

                DDLDTO ddldata = FillDD_Env_Parameter(projmodel.ddlData);
               
                response = client.PostAsJsonAsync(cWebApiNames.APIGetProjectDetails, projmodel).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    projmodel = JsonConvert.DeserializeObject<ProjectViewModel>(responseMsg);
                    if (ddldata.opStatus)
                    {
                        projmodel.SLEnvironment = new SelectList(ddldata.Data.ExtractDDLDataForKey(DBTableNameEnums.EnvironmentMaster), "Value", "Text", projmodel.env_syscode);
                    }
                }
                else
                {
                    throw new Exception(responseMsg);
                }
            }
            catch (Exception ex)
            {
                 ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "Post_ViewProjectDetails", "ProjectController");
                return Content("Error Occured: " + ex.Message);

            }
            return PartialView("~/Areas/Tasks/Views/Task/_ReleaseDetails.cshtml", projmodel);
        }
        #endregion

    }
  }