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
using Task_Tracker_Solution.Areas.Common.Controllers;
using Task_Tracker_Solution.Utility;

namespace Task_Tracker_Solution.Areas.Dashboard.Controllers
{
    public class DashboardController : TaskTrackerBaseController
    {
        // GET: Dashboard/Dashboard
        public ActionResult MyTaskActivity()
        {
            IList<TaskDM> viewmodel = null;
            try
            {
                string sreturn = string.Empty;

                if (this.Session["emp_syscode"] != null)
                {
                    int loggedin_user = Convert.ToInt32(Convert.ToString(this.Session["emp_syscode"]));

                    TaskUserMapping taskUser = new TaskUserMapping();
                    taskUser.employee_syscode = loggedin_user;

                    var response = client.PostAsJsonAsync(cWebApiNames.APIGetMyTaskActivity, taskUser).Result;
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
                else
                {
                    return RedirectToAction("Logout", "Login", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.ErrorMessage = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "MyTasks", "TaskController");
            }

            return View(viewmodel);
        }

        public ActionResult OpenTasks()
        {           
            IList<TaskDM> viewmodel = null;
            try
            {
                string sreturn = string.Empty;
               
                if (this.Session["emp_syscode"] != null)
                {
                    int loggedin_user = Convert.ToInt32(Convert.ToString(this.Session["emp_syscode"]));

                    TaskUserMapping taskUser = new TaskUserMapping();
                    taskUser.employee_syscode = loggedin_user;

                    var response = client.PostAsJsonAsync(cWebApiNames.APIGetTodaysLatestTasks, taskUser).Result;
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
                else
                {
                    return RedirectToAction("Logout", "Login", new { Area = ""});
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.ErrorMessage = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "MyTasks", "TaskController");
            }

            return View(viewmodel);
        }

        public ActionResult TeamActivity(DashBoard taskuser)
        {
            DashBoard viewmodel = null;
            try
            {
                HttpCookie TeamActivityCookie = HttpContext.Request.Cookies.Get("TeamActivityData");
                if (TeamActivityCookie != null)
                {
                    string cookieValue = TeamActivityCookie.Value;
                    if (!string.IsNullOrEmpty(cookieValue))
                    {
                        string decodedValue = HttpUtility.UrlDecode(cookieValue);
                        Dictionary<string, string> jsonDeserializedVal = JsonConvert.DeserializeObject<Dictionary<string, string>>(decodedValue);
                        foreach (var ckey in jsonDeserializedVal.Keys)
                        {
                            DateTime outdt;
                            DateTime? dt = DateTime.TryParse(jsonDeserializedVal[ckey], out outdt) ? outdt : default(DateTime?);
                            switch (ckey)
                            {
                                case "StartDate":
                                    taskuser.startdate = dt;
                                    break;
                                case "EndDate":
                                    taskuser.enddate = dt;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                taskuser.logged_in_user = ssLoggedInEmpSyscode;
                taskuser.logged_in_user_name = ssLoggedInEmpName;
                taskuser.ed_syscode = ssLoggedInEmpSyscode;
                taskuser.group_syscode = ssGroupSyscode;
                var response = client.PostAsJsonAsync(cWebApiNames.APIGetTeamActivity, taskuser).Result;
                var responseMsg = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    viewmodel = JsonConvert.DeserializeObject<DashBoard>(responseMsg);
                    viewmodel.startdate = taskuser.startdate;
                    viewmodel.enddate = taskuser.enddate;
                    viewmodel.JsonProjectUsersData = JsonConvert.SerializeObject(viewmodel.lstUsersProjects);
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ViewBag.ErrorMessage = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "TeamActivity", "DashboardController");
            }
            return View("TeamActivity", viewmodel);
        }
        public ActionResult GetAdminDashboard(DashBoard taskuser)
        {
            DashBoard viewmodel = null;

            try
            {
                if (this.Session["emp_syscode"] != null)
                {
                    int loggedin_user = Convert.ToInt32(Convert.ToString(this.Session["emp_syscode"]));
                    // loggedin_user = 3986;
                    taskuser.ed_syscode = loggedin_user;
                    taskuser.group_syscode = ssGroupSyscode;
                    var response = client.PostAsJsonAsync(cWebApiNames.APIGetDashboard, taskuser).Result;
                    var responseMsg = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        viewmodel = JsonConvert.DeserializeObject<DashBoard>(responseMsg);
                        viewmodel.JsonProjectUsersData = JsonConvert.SerializeObject(viewmodel.lstUsersProjects);
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
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
                ViewBag.ErrorMessage = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "GetAdminDashboard", "DashboardController");
            }

            return View("Dashboard",viewmodel);
        }

       //[HttpPost]
       //public ActionResult GetfilteredData(DateTime? startdate, DateTime? enddate)
       // {
       //     DashBoard viewmodel = null;
       //     try
       //     {
       //         string sreturn = string.Empty;

       //         if (this.Session["emp_syscode"] != null)
       //         {
       //             int loggedin_user = Convert.ToInt32(Convert.ToString(this.Session["emp_syscode"]));
       //             // loggedin_user = 3986;

       //             TaskUserMapping taskUser = new TaskUserMapping();
       //             taskUser.employee_syscode = loggedin_user;

       //             var response = client.PostAsJsonAsync(cWebApiNames.APIGetDashboard, taskUser).Result;
       //             var responseMsg = response.Content.ReadAsStringAsync().Result;
       //             if (response.IsSuccessStatusCode)
       //             {
       //                 viewmodel = JsonConvert.DeserializeObject<DashBoard>(responseMsg);
       //                 viewmodel = JsonConvert.DeserializeObject<DashBoard>(responseMsg);
       //                 if(viewmodel != null)
       //                 {
       //                     var data = viewmodel.lstUsersProjects.FindAll(a=> a.)
       //                     viewmodel.lstUsersProjects.Select
       //                 }
       //             }
       //             else
       //             {
       //                 throw new Exception(response.ReasonPhrase);
       //             }
       //         }
       //         else
       //         {
       //             return RedirectToAction("Logout", "Login", new { Area = "" });
       //         }
       //     }
       //     catch (Exception ex)
       //     {
       //         TempData["ErrorMessage"] = ex.Message;
       //         ViewBag.ErrorMessage = ex.Message;
       //         ModelState.AddModelError("KeyException", ex.Message);
       //         Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "MyTasks", "TaskController");
       //     }

       //     return View("Dashboard", viewmodel);
       // } 

    }
}