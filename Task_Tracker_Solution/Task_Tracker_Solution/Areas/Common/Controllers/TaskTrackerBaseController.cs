using Common_Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Solution.Utility;
using System.Web.Mvc.Filters;
using Task_Tracker_Solution.Controllers;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Solution.Areas.Common.Controllers
{
    public class TaskTrackerBaseController : BaseWebController
    {
        public TaskTrackerBaseController()
        {
        }

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            if (!IsSessionValid)
            {
                filterContext.Result = this.RedirectToAction(nameof(LoginController.Logout), "Login", new { Area = "" });
            }
            base.OnAuthentication(filterContext);
        }

        protected override void Initialize(RequestContext requestContext)
        {
            IsSessionValid = (bool)(requestContext.HttpContext.Session["user_session"] ?? false);

            base.Initialize(requestContext);
            if (IsSessionValid)
            {
                ssLoggedInEmpSyscode = Convert.ToInt32(Convert.ToString(requestContext.HttpContext.Session["emp_syscode"]));
                if (requestContext.HttpContext.Session["group_syscode"] != null)
                {
                    ssGroupSyscode = Convert.ToInt32(Convert.ToString(requestContext.HttpContext.Session["group_syscode"]));
                }
                ssLoggedInEmpName = Convert.ToString(requestContext.HttpContext.Session["emp_name"]);

                string _Token = Convert.ToString(requestContext.HttpContext.Session["Token"]);//Get Token from Session
                client.DefaultRequestHeaders.Add("Token", _Token);//Set Token in Header
            }
            //else
            //{
            //    requestContext.HttpContext.Response.Clear();
            //    requestContext.HttpContext.Response.Redirect(Url.Action("Logout", "Login"));
            //    requestContext.HttpContext.Response.End();
            //    //return RedirectToAction("Logout", "Login", new { Area = "" });
            //}
        }

        /// <summary>
        /// This is getting called using AJAX request from Group drop down change event.
        /// </summary>
        /// <param name="selectedGroupSyscode">It is passes as a get request.</param>
        /// <returns></returns>
        public bool ChangeGroup(string selectedGroupSyscode)
        {
            try
            {
                if (!string.IsNullOrEmpty(selectedGroupSyscode))
                {
                    int selGroupSyscode = Convert.ToInt32(selectedGroupSyscode);
                    SelectList sl = (SelectList)Session["GroupDDLSL"];
                                        
                    if (!sl.Any(x=> x.Value == selectedGroupSyscode))
                    {
                        throw new Exception($"Selected group {selectedGroupSyscode} is not present in the group list of the user {ssLoggedInEmpName}");
                    }
                    ssGroupSyscode = selGroupSyscode;                    
                    Session["group_syscode"] = ssGroupSyscode;
                    SelectList sl1 = new SelectList(sl.Items, "Value", "Text", ssGroupSyscode);
                    Session["GroupDDLSL"] = sl1;
                    AddGroupToCookie(ssGroupSyscode);
                    return true;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                ModelState.AddModelError("KeyException", ex.Message);
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "ChangeGroup", "TaskTrackerBase");
            }
            return false;//RedirectToAction("MyTaskActivity", "Dashboard", new { Area = "Dashboard" });
        }

        protected SearchDTO FillSearchDTO(SearchDTO _searchCriteria)
        {
            ///ToDo: Find a way to add the whole search DTO in cookie using the controller function and not from java script.
            #region Look for filter data in TempData
            if (TempData["ObjSearchDTO"] != null) //This condition is utilised for navigating the search conditions from the "Create Project Task" page to "Assign Task" page.
            {
                SearchDTO srTD = (SearchDTO)TempData["ObjSearchDTO"];
                _searchCriteria.project_syscode = srTD.project_syscode;
                _searchCriteria.module_syscode = srTD.module_syscode;
                _searchCriteria.category_syscode = srTD.category_syscode;
                _searchCriteria.task_syscode = srTD.task_syscode;
                _searchCriteria.status_syscode = srTD.status_syscode;
                AddToCookie("TaskFilterData", srTD);
            }
            else
            {
                #endregion
                //Below assignment from cookie works even during the time when user changes the values in the search control and presses
                //Search, as the newly selected values are first replaced in the cookie and then the request hits the controller action.
                #region Look for Filter in Cookie
                HttpCookie FilterDataCookie = HttpContext.Request.Cookies.Get("TaskFilterData");
                if (FilterDataCookie != null)
                {
                    string cookieValue = FilterDataCookie.Value;
                    if (!string.IsNullOrEmpty(cookieValue))
                    {
                        string decodedValue = HttpUtility.UrlDecode(cookieValue);
                        Dictionary<string, string> jsonDeserializedVal = JsonConvert.DeserializeObject<Dictionary<string, string>>(decodedValue);
                        foreach (var ckey in jsonDeserializedVal.Keys)
                        {
                            int sysCode = int.TryParse(jsonDeserializedVal[ckey], out sysCode) ? sysCode : 0;
                            switch (ckey)
                            {
                                case "project":
                                    _searchCriteria.project_syscode = sysCode;
                                    break;
                                case "category":
                                    _searchCriteria.category_syscode = sysCode;
                                    break;
                                case "module":
                                    _searchCriteria.module_syscode = sysCode;
                                    break;
                                case "task":
                                    _searchCriteria.task_syscode = sysCode;
                                    break;
                                case "status":
                                    _searchCriteria.status_syscode = sysCode;
                                    break;
                                default:
                                    break;
                            }
                        }
                        //_searchCriteria.project_syscode = int.TryParse(jsonDeserializedVal["project"], out projSyscode) ? projSyscode : 0;
                        //_searchCriteria.category_syscode = int.TryParse(jsonDeserializedVal["category"], out catSyscode) ? catSyscode : 0;
                        //_searchCriteria.module_syscode = int.TryParse(jsonDeserializedVal["module"], out modSyscode) ? modSyscode : 0;
                        //_searchCriteria.task_syscode = int.TryParse(jsonDeserializedVal["task"], out taskSyscode) ? taskSyscode : 0;
                    }
                }
            }
            #endregion

            #region Prepare DDLData obj for fetching it from API
            _searchCriteria.ddlData.Predicate[DBTableNameEnums.ProjectMaster]["GetData"] = true;
            _searchCriteria.ddlData.Predicate[DBTableNameEnums.CategoryMaster]["GetData"] = true;
            _searchCriteria.ddlData.Predicate[DBTableNameEnums.CategoryMaster].Add(nameof(CategoryMaster.group_syscode), ssGroupSyscode);
            _searchCriteria.ddlData.Predicate[DBTableNameEnums.ProjectMaster].Add(nameof(CategoryMaster.group_syscode), ssGroupSyscode);
            _searchCriteria.ddlData.Predicate[DBTableNameEnums.TaskStatusMaster]["GetData"] = true;

            if (_searchCriteria.project_syscode > 0 || _searchCriteria.category_syscode > 0)
            {
                _searchCriteria.ddlData.Predicate[DBTableNameEnums.ModuleMaster]["GetData"] = true;

                if (_searchCriteria.project_syscode > 0)
                    _searchCriteria.ddlData.Predicate[DBTableNameEnums.ModuleMaster].Add(nameof(ModuleMaster.project_syscode), _searchCriteria.project_syscode);
                if (_searchCriteria.category_syscode > 0)
                    _searchCriteria.ddlData.Predicate[DBTableNameEnums.ModuleMaster].Add(nameof(ModuleMaster.category_syscode), _searchCriteria.category_syscode);

                if (_searchCriteria.module_syscode > 0)
                {
                    _searchCriteria.ddlData.Predicate[DBTableNameEnums.Tasks]["GetData"] = true;
                    _searchCriteria.ddlData.Predicate[DBTableNameEnums.Tasks].Add(nameof(ModuleMaster.module_syscode), _searchCriteria.module_syscode);
                }
            }
            #endregion

            #region Fetch DropDownList From API
            var response = client.PostAsJsonAsync(cWebApiNames.APIGetDDLData, _searchCriteria.ddlData).Result;

            var responseMsg = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                _searchCriteria.ddlData = JsonConvert.DeserializeObject<DDLDTO>(responseMsg);
                if (_searchCriteria.ddlData.opStatus)
                {
                    _searchCriteria.SLProjects = new SelectList(_searchCriteria.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.ProjectMaster), "Value", "Text", _searchCriteria.project_syscode);
                    _searchCriteria.SLCategory = new SelectList(_searchCriteria.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.CategoryMaster), "Value", "Text", _searchCriteria.category_syscode);
                    _searchCriteria.SLModules = new SelectList(_searchCriteria.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.ModuleMaster), "Value", "Text", _searchCriteria.module_syscode);
                    _searchCriteria.SLTasks = new SelectList(_searchCriteria.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.Tasks), "Value", "Text", _searchCriteria.task_syscode);
                    _searchCriteria.SLStatus = new SelectList(_searchCriteria.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.TaskStatusMaster), "Value", "Text", _searchCriteria.status_syscode);

                }
            }
            else
                throw new Exception(response.ReasonPhrase);
            #endregion

            return _searchCriteria;
        }
        protected void AddToCookie(string _cookieName, SearchDTO _obj)
        {            
            if (string.IsNullOrEmpty(_cookieName) || _obj == null)
            {
                return;
            }
            string objString = "{'project':'" + _obj.project_syscode + "', 'category':'" + _obj.category_syscode + "', 'module':'" + _obj.module_syscode + "', 'task':'" + _obj.task_syscode + "', 'status':'" + _obj.status_syscode + "'}"; ; //JsonConvert.SerializeObject(_obj);
            HttpCookie thisCookie = new HttpCookie(_cookieName);
            thisCookie.Value = objString;//ssGroupSyscode.ToString();
            thisCookie.HttpOnly = true;
            thisCookie.Expires = DateTime.Now.AddDays(5);
            Response.Cookies.Add(thisCookie);
        }
    }
}