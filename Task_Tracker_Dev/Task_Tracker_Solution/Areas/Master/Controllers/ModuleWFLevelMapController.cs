using Common_Components;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
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
    public class ModuleWFLevelMapController : TaskTrackerBaseController
    {
        public ModuleWFLevelMapController()
        {

        }
        // GET: Master/ModuleWFLevelMap
        public ActionResult GetModuleWFLevelMap(SearchDTO _searchDTO)
        {
            ModuleWFLevelMapViewModel viewmodel = null;
            try
            {
                viewmodel = new ModuleWFLevelMapViewModel();
                _searchDTO = FillSearchDTO(_searchDTO);
                _searchDTO.enableSearchTextBox = false;
                _searchDTO.enableTaskDD = false;
                viewmodel.searchDTO = _searchDTO;

                #region Load Workflow Levels on module selection
                if (_searchDTO.module_syscode > 0)
                {
                    viewmodel.logged_in_user = ssLoggedInEmpSyscode;
                    viewmodel.created_by = ssLoggedInEmpSyscode;
                    viewmodel.project_syscode = viewmodel.searchDTO.project_syscode;
                    viewmodel.category_syscode = viewmodel.searchDTO.category_syscode;
                    viewmodel.module_syscode = viewmodel.searchDTO.module_syscode;

                    viewmodel.ddlData.Predicate[DBTableNameEnums.GroupMember]["GetData"] = true;
                    viewmodel.ddlData.Predicate[DBTableNameEnums.GroupMember].Add(nameof(GroupMaster.group_syscode), ssGroupSyscode);

                    var response = client.PostAsJsonAsync(cWebApiNames.GetLevelTaskUsersList, viewmodel).Result;
                    var responseMsg = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        viewmodel = JsonConvert.DeserializeObject<ModuleWFLevelMapViewModel>(responseMsg);

                        viewmodel.project_syscode = _searchDTO.project_syscode;
                        viewmodel.category_syscode = _searchDTO.category_syscode;
                        viewmodel.module_syscode = _searchDTO.module_syscode;
                        viewmodel.searchDTO = _searchDTO;

                        if (viewmodel.opStatus)
                        {
                            viewmodel.SLEmployee = new MultiSelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.GroupMember), "Value", "Text");
                        }
                    }
                    else
                        throw new Exception(response.ReasonPhrase);
                } 
                #endregion              
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "Index", "ModuleWFLevelMapController");
            }
            return View("ModuleWFLevelMap", viewmodel);
        }

        [HttpPost]
        public ActionResult SaveModuleWFLevel(ModuleWFLevelMapViewModel viewmodel, string Action)
        {
            try
            {
                if (viewmodel == null)
                    throw new ArgumentNullException("viewmodel", "Model cannot be null.");
               
                if (Action == "Save" || Action == "SaveContinue")
                {
                    SearchDTO _searchDTO = new SearchDTO();
                    _searchDTO.project_syscode = viewmodel.project_syscode;
                    _searchDTO.category_syscode = viewmodel.category_syscode;
                    _searchDTO.module_syscode = viewmodel.module_syscode;

                    _searchDTO = FillSearchDTO(_searchDTO);
                    _searchDTO.enableSearchTextBox = false;
                    _searchDTO.enableTaskDD = false;
                    _searchDTO.enableStatusDD = false;

                    viewmodel.created_by = ssLoggedInEmpSyscode;
                    viewmodel.created_by_Name = ssLoggedInEmpName;
                    viewmodel.ddlData.Predicate[DBTableNameEnums.GroupMember]["GetData"] = true;
                    viewmodel.ddlData.Predicate[DBTableNameEnums.GroupMember].Add(nameof(GroupMaster.group_syscode), ssGroupSyscode);

                    var response = client.PostAsJsonAsync(cWebApiNames.APIAddWFLevelMapping, viewmodel).Result;
                    var responseMsg = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        viewmodel = JsonConvert.DeserializeObject<ModuleWFLevelMapViewModel>(responseMsg);
                        if (viewmodel.opStatus)
                        {
                            viewmodel.project_syscode = _searchDTO.project_syscode;
                            viewmodel.module_syscode = _searchDTO.category_syscode;
                            viewmodel.category_syscode = _searchDTO.module_syscode;
                            viewmodel.searchDTO = _searchDTO;

                            viewmodel.SLEmployee = new MultiSelectList(viewmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.GroupMember), "Value", "Text");

                            ViewBag.SuccessMessage = "Record saved sucessfully.";
                            
                            if (Action == "SaveContinue")
                            {                                
                                TempData["ObjSearchDTO"] = _searchDTO;
                                return RedirectToAction("CreateProjectTask", "ProjectTask", new { Area = "Tasks" });
                            }
                        }
                        else
                            throw new Exception(viewmodel.opMsg);
                    }
                    else
                        throw new Exception(response.ReasonPhrase);
                }

                ModelState.Clear();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "GetModules", "ModuleWFLevelMapController");
            }
            return View("ModuleWFLevelMap", viewmodel);
        }        
    }
}