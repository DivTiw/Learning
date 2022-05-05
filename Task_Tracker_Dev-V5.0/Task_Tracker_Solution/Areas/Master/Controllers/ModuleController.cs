using Common_Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Solution.Areas.Common.Controllers;
using Task_Tracker_Solution.Areas.Master.Models;
using Task_Tracker_Solution.Utility;

namespace Task_Tracker_Solution.Areas.Master
{
    public class ModuleController : TaskTrackerBaseController
    {
        // GET: Master/Module
        
        public ActionResult ProjectModules(string id = null, string projname = null)
        {
            //IList<ModuleViewModel> lstModulemodel = null;
           ModuleViewModel modmodel = null;
            try
            {
                string sreturn = string.Empty;
                int proj_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));

                modmodel = new ModuleViewModel();
                modmodel.logged_in_user = ssLoggedInEmpSyscode;
                modmodel.project_syscode = proj_syscode;
                modmodel.project_name = projname;
             

                  var response = client.PostAsJsonAsync(cWebApiNames.APIGetProjectModuleList, modmodel).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    modmodel.lstModuleDM = JsonConvert.DeserializeObject<List<ModuleDM>>(responseMsg);
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
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");
            }
            return View(modmodel);
        }

        public ActionResult CreateModule(string id = null, string projname = null)
        {
            ModuleViewModel modulemodel = null;
            try
            {
                ViewBag.Title = "Create Module";
                string sreturn = string.Empty;
                int proj_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));

                if (proj_syscode <= 0)
                {
                    throw new Exception("Invalid Project");
                }

                modulemodel = new ModuleViewModel();

                modulemodel.logged_in_user = ssLoggedInEmpSyscode;
                modulemodel.project_name = projname;
                modulemodel.project_syscode = proj_syscode;
                string predcName = nameof(modulemodel.group_syscode); //"group_syscode";//

                modulemodel.ddlData.Predicate[DBTableNameEnums.GroupMember]["GetData"] = true;
                modulemodel.ddlData.Predicate[DBTableNameEnums.GroupMember].Add(predcName, ssGroupSyscode);
                modulemodel.ddlData.Predicate[DBTableNameEnums.CategoryMaster]["GetData"] = true;
                modulemodel.ddlData.Predicate[DBTableNameEnums.CategoryMaster].Add(predcName, ssGroupSyscode);
                modulemodel.ddlData.Predicate[DBTableNameEnums.WorkflowMaster]["GetData"] = true;
                modulemodel.ddlData.Predicate[DBTableNameEnums.WorkflowMaster].Add(predcName, ssGroupSyscode);

                DDLDTO ddldata = modulemodel.ddlData;

                var response = client.PostAsJsonAsync(cWebApiNames.APIGetDDLData, ddldata).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    modulemodel.ddlData = JsonConvert.DeserializeObject<DDLDTO>(responseMsg);
                    if (modulemodel.ddlData.opStatus)
                    {
                        modulemodel.SLWorkflowList = modulemodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.WorkflowMaster);
                        modulemodel.SLGroupMembers = modulemodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.GroupMember);
                        modulemodel.SLCategory = modulemodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.CategoryMaster);
                        modulemodel.MemberListJson = JsonConvert.SerializeObject(modulemodel.SLGroupMembers);
                        modulemodel.CategoryListJson = JsonConvert.SerializeObject(modulemodel.SLCategory);
                        modulemodel.WorkflowListJson = JsonConvert.SerializeObject(modulemodel.SLWorkflowList);
                        modulemodel.PageHasWriteAccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");
            }

            return View(modulemodel);
        }

        public ActionResult EditModule(string id = null, string projname = null)
        {
            ModuleViewModel modmodel = new ModuleViewModel();
            try
            {
                ViewBag.Title = "Edit Module";
                string sreturn = string.Empty;
                int module_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));
                string predcName = nameof(GroupDM.group_syscode);//"group_syscode"; //nameof(projmodel.group_syscode);

                if (module_syscode > 0) //Edit Module
                {
                    modmodel = new ModuleViewModel();
                    modmodel.logged_in_user = ssLoggedInEmpSyscode;
                    modmodel.module_syscode = module_syscode;
                    modmodel.project_name = projname;
                    modmodel.group_syscode = ssGroupSyscode;

                    modmodel.ddlData.Predicate[DBTableNameEnums.GroupMember]["GetData"] = true;
                    modmodel.ddlData.Predicate[DBTableNameEnums.GroupMember].Add(predcName, ssGroupSyscode);
                    modmodel.ddlData.Predicate[DBTableNameEnums.CategoryMaster]["GetData"] = true;
                    modmodel.ddlData.Predicate[DBTableNameEnums.CategoryMaster].Add(predcName, ssGroupSyscode);
                    modmodel.ddlData.Predicate[DBTableNameEnums.WorkflowMaster]["GetData"] = true;

                    var response = client.PostAsJsonAsync(cWebApiNames.APIGetModuleByID, modmodel).Result;
                    var responseMsg = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        modmodel = JsonConvert.DeserializeObject<ModuleViewModel>(responseMsg);
                        if (modmodel.opStatus)
                        {
                            modmodel.project_syscode = modmodel.project_syscode;
                            modmodel.SLWorkflowList = modmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.WorkflowMaster);
                            modmodel.SLGroupMembers = modmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.GroupMember);
                            modmodel.SLCategory = modmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.CategoryMaster);
                            modmodel.MemberListJson = JsonConvert.SerializeObject(modmodel.SLGroupMembers);
                            modmodel.CategoryListJson = JsonConvert.SerializeObject(modmodel.SLCategory);
                            modmodel.WorkflowListJson = JsonConvert.SerializeObject(modmodel.SLWorkflowList);
                            modmodel.project_name = projname;
                            modmodel.module_syscode = modmodel.module_syscode;
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
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");
            }

            return View("CreateModule", modmodel);
        }
        [HttpPost]
        public async Task<ActionResult> Submit(ModuleViewModel modulemodel)
        {
            try
            {
                string sreturn = string.Empty;
                var response = new HttpResponseMessage();

                string predcName = nameof(modulemodel.group_syscode);//"group_syscode";//

                modulemodel.logged_in_user = ssLoggedInEmpSyscode;
                modulemodel.created_by = ssLoggedInEmpSyscode;
                modulemodel.group_syscode = ssGroupSyscode;

                modulemodel.ddlData.Predicate[DBTableNameEnums.GroupMember]["GetData"] = true;
                modulemodel.ddlData.Predicate[DBTableNameEnums.GroupMember].Add(predcName, ssGroupSyscode);
                modulemodel.ddlData.Predicate[DBTableNameEnums.CategoryMaster]["GetData"] = true;
                modulemodel.ddlData.Predicate[DBTableNameEnums.CategoryMaster].Add(predcName, ssGroupSyscode);
                modulemodel.ddlData.Predicate[DBTableNameEnums.WorkflowMaster]["GetData"] = true;

                if (!string.IsNullOrEmpty(modulemodel.MemberListJson))
                {
                    modulemodel.SLGroupMembers = JsonConvert.DeserializeObject<List<SelectItemDTO>>(modulemodel.MemberListJson);
                }
                if (!string.IsNullOrEmpty(modulemodel.CategoryListJson))
                {
                    modulemodel.SLCategory = JsonConvert.DeserializeObject<List<SelectItemDTO>>(modulemodel.CategoryListJson);
                }
                if (!string.IsNullOrEmpty(modulemodel.WorkflowListJson))
                {
                    modulemodel.SLWorkflowList = JsonConvert.DeserializeObject<List<SelectItemDTO>>(modulemodel.WorkflowListJson);
                }

                if (string.IsNullOrEmpty(modulemodel.module_name))
                {
                    throw new Exception("Please enter Module Name");
                }

                if (ComLibCommon.CheckDuplicateUser(modulemodel.arrReadUserSyscodes, modulemodel.arrWriteUserSyscodes))
                {
                    throw new Exception("Read Access User cannot be added as Write Access User.");
                }

                if (modulemodel.module_syscode > 0)
                {
                    modulemodel.modified_by = ssLoggedInEmpSyscode;
                    response = await client.PutAsJsonAsync(Utility.cWebApiNames.APIPutModule, modulemodel);
                }
                else
                {
                    response = await client.PostAsJsonAsync(Utility.cWebApiNames.APIPostModule, modulemodel);
                }

                if (response.IsSuccessStatusCode)
                {
                    var Response = response.Content.ReadAsStringAsync().Result;
                    OperationDetailsDTO od = JsonConvert.DeserializeObject<OperationDetailsDTO>(Response);

                    if (od.opStatus)
                    {
                        ModelState.Clear();
                        modulemodel.project_syscode = modulemodel.project_syscode;
                        modulemodel.opMsg = od.opMsg;

                        ViewBag.SuccessMessage = od.opMsg;

                        return RedirectToAction("ProjectModules", new { id = ComLibCommon.Base64Encode(modulemodel.project_syscode + ""), projname = modulemodel.project_name});//+"")
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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "Submit", "WorkflowController");
                if (modulemodel.module_syscode > 0)
                {
                    ViewBag.Title = "Edit Module";
                }
                else
                {
                    ViewBag.Title = "Create Module";
                }
                return View("CreateModule", modulemodel);
            }
        }
    }
}