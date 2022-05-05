using Common_Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Solution.Areas.Common.Controllers;
using Task_Tracker_Solution.Areas.Master.Models;
using Task_Tracker_Solution.Utility;

namespace Task_Tracker_Solution.Areas.Reports.Controllers
{
    public class ReportController : TaskTrackerBaseController
    {
        // GET: Reports/Report
        public ActionResult ReleaseReport(string id = null)
        {
            ReleaseViewModel viewmodel = null;
            try
            {
                int task_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));              

                viewmodel = new ReleaseViewModel();
                viewmodel.task_syscode = task_syscode;
                viewmodel.logged_in_user = ssLoggedInEmpSyscode;

                var response = client.PostAsJsonAsync(cWebApiNames.APIGetTaskReleaseReport, viewmodel).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    viewmodel.lstReleaseDM = JsonConvert.DeserializeObject<IList<ReleaseDM>>(responseMsg);
                    foreach(var item in viewmodel.lstReleaseDM)
                    {
                        item.release_params_json = JsonConvert.SerializeObject(item.lstReleaseDetailsDM);
                    }
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
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "ReleaseReport", "ReportController");
            }

            return View(viewmodel);
        }


        [HttpPost]
        public ActionResult ViewReleaseParameters(string id, string paramsJSON)
        {
            ReleaseViewModel viewmodel = null;
            try
            {
                int release_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));

                if (release_syscode <= 0)
                    throw new Exception("Please select Release to view Parameters.");

                viewmodel = new ReleaseViewModel();
                viewmodel.release_syscode = release_syscode;
                viewmodel.logged_in_user = ssLoggedInEmpSyscode;

                ///ToDo: API here

                if (!String.IsNullOrEmpty(paramsJSON))
                {
                    viewmodel.lstReleaseDetailsDM = JsonConvert.DeserializeObject<IList<ReleaseDetailsDM>>(paramsJSON);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "ReleaseReport", "ReportController");
            }

            return PartialView("~/Areas/Reports/Views/Report/_ReleaseParameters.cshtml", viewmodel); 
        }
     


        [HttpPost]
        public ActionResult UpdateRelease(ReleaseViewModel viewmodel)
        {
            try
            {                
                if (viewmodel == null || viewmodel.release_syscode <= 0)
                    throw new Exception("Invalid Release.");

                viewmodel.logged_in_user = ssLoggedInEmpSyscode;
                viewmodel.logged_in_user_name = ssLoggedInEmpName;

                var response = client.PostAsJsonAsync(cWebApiNames.APIUpdateRelease, viewmodel).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    var od = JsonConvert.DeserializeObject<OperationDetailsDTO>(responseMsg);
                    if(od.opStatus)
                    {
                        return RedirectToAction("ReleaseReport", new { id = ComLibCommon.Base64Encode(viewmodel.task_syscode.ToString()) });
                    }
                    else                   
                        throw new Exception(od.opMsg, od.opInnerException);
                 
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
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "ReleaseReport", "ReportController");
            }
            return View("ReleaseReport");
        }
    }
}