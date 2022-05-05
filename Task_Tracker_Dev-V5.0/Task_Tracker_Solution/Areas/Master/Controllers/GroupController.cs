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

namespace Task_Tracker_Solution.Areas.Master.Controllers
{
    public class GroupController : TaskTrackerBaseController
    {
        // GET: Master/Group
        public ActionResult Index()
        {
            IList<GroupViewModel> grpmodel = null;
            try
            {
                GroupDM grpDm = new GroupDM();
                grpDm.logged_in_user = ssLoggedInEmpSyscode;

                var response = client.PostAsJsonAsync(cWebApiNames.APIGetAllGetAllGroups, grpDm).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    grpmodel = JsonConvert.DeserializeObject<IList<GroupViewModel>>(responseMsg);
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
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "Submit", "GroupController");
            }

            return View("GroupList", grpmodel);
        }
        public ActionResult CreateGroup()
        {
            GroupViewModel grpVmodel = null;
            try
            {
                grpVmodel = new GroupViewModel();

                grpVmodel.ddlData.Predicate[DBTableNameEnums.vw_employee_master]["GetData"] = true;

                DDLDTO ddldata = grpVmodel.ddlData;

                var response = client.PostAsJsonAsync(cWebApiNames.APIGetDDLData, ddldata).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    grpVmodel.ddlData = JsonConvert.DeserializeObject<DDLDTO>(responseMsg);
                    grpVmodel.mslGrpHeads = new MultiSelectList(new List<SelectListItem>() { });
                    grpVmodel.mslGrpMembers = new MultiSelectList(new List<SelectListItem>() { });

                    if (grpVmodel.ddlData.opStatus)
                    {
                        var empDDL = grpVmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.vw_employee_master);
                        grpVmodel.json_mslEmployeeList = JsonConvert.SerializeObject(empDDL);
                        grpVmodel.mslGrpHeads = new MultiSelectList(empDDL, "Value", "Text");
                        grpVmodel.mslGrpMembers = new MultiSelectList(empDDL, "Value", "Text");
                        grpVmodel.PageHasWriteAccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "CreateGroup", "GroupController");
            }

            return View(grpVmodel);
        }

        public ActionResult EditGroup(string id = null)
        {
            GroupViewModel grpVmodel = null;
            try
            {
                if (String.IsNullOrEmpty(id))
                {
                    throw new Exception("Group ID is not found in the link. Please contact IT.");
                }

                int grpSyscode = Convert.ToInt32(ComLibCommon.Base64Decode(id));

                grpVmodel = new GroupViewModel();
                grpVmodel.ddlData.Predicate[DBTableNameEnums.vw_employee_master]["GetData"] = true;

                grpVmodel.group_syscode = grpSyscode;
                grpVmodel.logged_in_user = ssLoggedInEmpSyscode;
                grpVmodel.created_by = ssLoggedInEmpSyscode;

                if (grpSyscode > 0)
                {
                    var response = client.PostAsJsonAsync(cWebApiNames.APIGetGroupByID, grpVmodel).Result;
                    var responseMsg = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        grpVmodel = JsonConvert.DeserializeObject<GroupViewModel>(responseMsg);
                        grpVmodel.mslGrpHeads = new MultiSelectList(new List<SelectListItem>() { });
                        grpVmodel.mslGrpMembers = new MultiSelectList(new List<SelectListItem>() { });

                        if (grpVmodel.ddlData.opStatus)
                        {
                            var empDDL = grpVmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.vw_employee_master);
                            grpVmodel.json_mslEmployeeList = JsonConvert.SerializeObject(empDDL);
                            grpVmodel.mslGrpHeads = new MultiSelectList(empDDL, "Value", "Text", grpVmodel.arrGrpHeadSyscodes);
                            grpVmodel.mslGrpMembers = new MultiSelectList(empDDL, "Value", "Text", grpVmodel.arrMemSyscodes);
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
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "Submit", "GroupController");
            }

            return View("CreateGroup", grpVmodel);
        }

        [HttpPost]
        public async Task<ActionResult> Submit(GroupViewModel grpmodel)
        {
            try
            {
                var response = new HttpResponseMessage();
                grpmodel.created_by = ssLoggedInEmpSyscode;
                grpmodel.logged_in_user = ssLoggedInEmpSyscode;
                List<SelectItemDTO> empSL = JsonConvert.DeserializeObject<List<SelectItemDTO>>(grpmodel.json_mslEmployeeList);
                grpmodel.mslGrpHeads = new MultiSelectList(empSL, "Value", "Text", grpmodel.arrGrpHeadSyscodes);
                grpmodel.mslGrpMembers = new MultiSelectList(empSL, "Value", "Text", grpmodel.arrMemSyscodes);

                #region "Save Group"

                if (string.IsNullOrEmpty(grpmodel.group_name) || string.IsNullOrEmpty(grpmodel.group_description))
                {
                    throw new Exception("Invalid Group data. Name or Description of the Group can not be null.");
                }

                if (grpmodel.group_syscode > 0)
                {
                    response = await client.PutAsJsonAsync(Utility.cWebApiNames.APIUpdateGroup, grpmodel);
                }
                else
                {
                    response = await client.PostAsJsonAsync(Utility.cWebApiNames.APIPostGroup, grpmodel);
                }

                if (response.IsSuccessStatusCode)
                {
                    var Response = response.Content.ReadAsStringAsync().Result;
                    GroupViewModel gvm = JsonConvert.DeserializeObject<GroupViewModel>(Response);
                    if (gvm.opStatus)
                    {
                        ViewBag.SuccessMessage = "Record saved successfully.";
                        SetGroupDDLData(ssLoggedInEmpSyscode);
                        return RedirectToAction("Index", "Group", new { Area = "Master" });
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error occurred while saving the record.";
                        throw new Exception(gvm.opMsg, gvm.opInnerException);
                    }
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "Submit", "GroupController");
            }
            return View("CreateGroup", grpmodel);
        }
    }
}