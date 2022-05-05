using Common_Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Solution.Areas.Common.Controllers;
using Task_Tracker_Solution.Areas.Master.Models;
using Task_Tracker_Solution.Utility;

namespace Task_Tracker_Solution.Areas.Master.Controllers
{
    public class WorkflowController : TaskTrackerBaseController
    {
        public WorkflowController()
        {
        }

        // GET: Master/Workflow
        public ActionResult CreateWorkflow(string id = null)
        {
            WorkflowViewmodel wfmodel = null;
            try
            {
                string sreturn = string.Empty;
                if (this.Session["emp_syscode"] != null)
                {
                    int loggedin_user = Convert.ToInt32(Convert.ToString(this.Session["emp_syscode"]));
                    int wf_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));

                    wfmodel = new WorkflowViewmodel();
                    wfmodel.workflow_syscode = wf_syscode;

                    if (wf_syscode > 0)
                    {
                        var response = client.PostAsJsonAsync(cWebApiNames.APIGetWorkflowByID, wfmodel).Result;
                        var responseMsg = response.Content.ReadAsStringAsync().Result;
                        if (response.IsSuccessStatusCode)
                        {
                            wfmodel = JsonConvert.DeserializeObject<WorkflowViewmodel>(responseMsg);
                            wfmodel.lstWFLevelsJson = JsonConvert.SerializeObject(wfmodel.lstWFLevels);
                        }
                        else
                        {
                            throw new Exception(responseMsg);
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Login", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");
            }

            return View(wfmodel);
        }

        [HttpPost]
        public async Task<ActionResult> Submit(WorkflowViewmodel wfmodel, string action)
        {
            try
            {

                string sreturn = string.Empty;
                if (this.Session["emp_syscode"] != null)
                {
                    var response = new HttpResponseMessage();
                    int emp_syscode = int.Parse(Session["emp_syscode"].ToString());
                    wfmodel.created_by = emp_syscode;
                    wfmodel.group_syscode = ssGroupSyscode;

                    if (!string.IsNullOrEmpty(wfmodel.lstWFLevelsJson))
                    {
                        wfmodel.lstWFLevels = JsonConvert.DeserializeObject<IList<WorkflowLevelDetails>>(wfmodel.lstWFLevelsJson);
                    }

                    #region "Save WorkflowName"
                    if (action == "save_workflow")
                    {
                        if (string.IsNullOrEmpty(wfmodel.workflow_name))
                        {
                            throw new Exception("Please enter Workflow Name");
                        }

                        if (wfmodel.workflow_syscode > 0)
                        {
                            response = await client.PutAsJsonAsync(Utility.cWebApiNames.APIUpdateWorkflow, wfmodel);
                        }
                        else
                        {
                            wfmodel.lstWFLevels = null; //During first time workflow creation, Levels will always be null.
                            response = await client.PostAsJsonAsync(Utility.cWebApiNames.APIPostWorkflow, wfmodel);
                        }                        

                        if (response.IsSuccessStatusCode)
                        {
                            var Response = response.Content.ReadAsStringAsync().Result;
                            wfmodel = JsonConvert.DeserializeObject<WorkflowViewmodel>(Response);
                            if (wfmodel.opStatus)
                            {
                                wfmodel.lstWFLevelsJson = JsonConvert.SerializeObject(wfmodel.lstWFLevels);
                                ViewBag.SuccessMessage = "Record saved successfully.";
                                return RedirectToAction("WorkflowList");
                            }
                            else
                            {                                
                                throw new Exception(wfmodel.opMsg, wfmodel.opInnerException);
                            }
                        }
                        else
                        {
                            throw new Exception(response.ReasonPhrase);
                        }
                    }
                    else if (action == "copy_workflow")
                    {
                        if (wfmodel.workflow_syscode == 0)
                        {
                            throw new Exception("Invalid Parent Workflow.");
                        }                       

                        response = client.PostAsJsonAsync(cWebApiNames.APICopyWorkflow, wfmodel).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var responseMsg = response.Content.ReadAsStringAsync().Result;
                            WorkflowMaster wfm = JsonConvert.DeserializeObject<WorkflowMaster>(responseMsg);
                            if (wfm.opStatus)
                            {
                                wfmodel.workflow_syscode = wfm.workflow_syscode;
                                wfmodel.lstWFLevels = wfm.lstWFLevels;
                                wfmodel.lstWFLevelsJson = JsonConvert.SerializeObject(wfm.lstWFLevels);
                                ViewBag.SuccessMessage = wfm.opMsg;
                                return RedirectToAction("CreateWorkflow", new { id = ComLibCommon.Base64Encode(wfmodel.workflow_syscode + "") });
                            }
                            else
                            {                               
                                throw new Exception(wfm.opMsg);
                            }
                        }
                        else
                        {
                            throw new Exception(response.ReasonPhrase);
                        }

                    }
                    #endregion
                    #region "Save Workflow Levels"
                    else
                    {
                        if (wfmodel.lstWFLevels.Count > 0 && wfmodel.workflow_syscode > 0 && !wfmodel.lstWFLevels.Any(x => x.workflow_syscode <= 0))
                        {
                            if (wfmodel.lstWFLevels.Any(m => m.level_order <= 0 || string.IsNullOrEmpty(m.level_name)))
                            {
                                throw new Exception("Please check if you have entered the level name and entered level order greater than zero.");
                            }
                            if (wfmodel.lstWFLevels.Any(x => wfmodel.lstWFLevels.Count(y => y.level_name == x.level_name || y.level_order == x.level_order) > 1)) //Join(projmodel.lstModules, mod=> mod.module_name, modCpy=> modCpy.module_name, (mod, modCpy)=> mod.module_name).Any()
                            {
                                throw new Exception("Please enter unique level names and level order in the list.");
                            }
                            response = await client.PostAsJsonAsync(Utility.cWebApiNames.APIAddUpdateWorkflowLevels, wfmodel);//lstWFLevels
                            if (response.IsSuccessStatusCode)
                            {
                                var Response = response.Content.ReadAsStringAsync().Result;
                                WorkflowMaster wfm = JsonConvert.DeserializeObject<WorkflowMaster>(Response);
                                if (wfm.opStatus)
                                {
                                    wfmodel.lstWFLevels = wfm.lstWFLevels;
                                    wfmodel.lstWFLevelsJson = JsonConvert.SerializeObject(wfmodel.lstWFLevels);
                                    ViewBag.SuccessMessage = "Record saved successfully.";
                                    return RedirectToAction("CreateWorkflow", new { id = ComLibCommon.Base64Encode(wfmodel.workflow_syscode + "") });
                                }
                                else
                                {
                                    ViewBag.SuccessMessage = "Error occurred while saving the record.";
                                    throw new Exception(wfm.opMsg);
                                }
                            }
                            else
                            {
                                throw new Exception(response.ReasonPhrase);
                            }
                        }
                        else
                            throw new Exception("Some problem occurred, check if empty list of levels is not submitted!");
                    }
                    #endregion
                }
                else
                {
                    return RedirectToAction("Logout", "Login", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");
                return View("CreateWorkflow", wfmodel);
            }
            //return View("CreateWorkflow", wfmodel);
        }

        public ActionResult WorkflowList()
        {
            IList<WorkflowViewmodel> lstwfmodel = null;
            try
            {
                string sreturn = string.Empty;
                if (this.Session["emp_syscode"] != null)
                {
                    WorkflowViewmodel wfmodel = new WorkflowViewmodel();
                    wfmodel.group_syscode = ssGroupSyscode;

                    var response = client.PostAsJsonAsync(cWebApiNames.APIGetAllWorkflowList, wfmodel).Result;
                    var responseMsg = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {

                        lstwfmodel = JsonConvert.DeserializeObject<IList<WorkflowViewmodel>>(responseMsg);
                    }
                    else
                    {
                        throw new Exception(responseMsg);
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Login", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");
            }

            return View("WorkflowList", lstwfmodel);

        }

        public ActionResult CopyWorkflow(WorkflowViewmodel wfmodel) //string id
        {
            //WorkflowViewmodel wfmodel = null;
            try
            {
                string sreturn = string.Empty;
                if (this.Session["emp_syscode"] != null)
                {
                    
                    //int wf_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));                

                    if (wfmodel.workflow_syscode == 0)
                    {
                        throw new Exception("Invalid Parent Workflow.");
                    }

                    //wfmodel = new WorkflowViewmodel();
                   // wfmodel.workflow_syscode = wf_syscode;
                    wfmodel.created_by = ssLoggedInEmpSyscode;
                    wfmodel.group_syscode = ssGroupSyscode;

                    var response = client.PostAsJsonAsync(cWebApiNames.APICopyWorkflow, wfmodel).Result;
                        var responseMsg = response.Content.ReadAsStringAsync().Result;
                        if (response.IsSuccessStatusCode)
                        {
                            wfmodel = JsonConvert.DeserializeObject<WorkflowViewmodel>(responseMsg);
                            wfmodel.lstWFLevelsJson = JsonConvert.SerializeObject(wfmodel.lstWFLevels);
                        }
                        else
                        {
                            throw new Exception(responseMsg);
                        }
                    
                }
                else
                {
                    return RedirectToAction("Logout", "Login", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");
            }

            return View("Create_Workflow", wfmodel);
        }

    }
}