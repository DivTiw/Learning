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
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Solution.Utility;

namespace Task_Tracker_Solution.Controllers
{
    public class BaseWebController : Controller
    {
        protected readonly HttpClient client = null;
        protected internal bool IsSessionValid { get; protected set; } = false;
        protected internal int ssGroupSyscode { get; protected set; }
        protected internal int ssLoggedInEmpSyscode { get; protected set; }
        protected internal string ssLoggedInEmpName { get; protected set; }
        public BaseWebController()
        {
            string web_apiAddress = ConfigurationManager.AppSettings["web_apiAddress"].ToString();

            client = new HttpClient();
            client.BaseAddress = new Uri(web_apiAddress);
            //client.Timeout = System.TimeSpan.FromMilliseconds(Convert.ToInt32(ConfigurationManager.AppSettings["ClientTimeout"]));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected int SetGroupDDLData(int empSyscode)
        {
            int selectedGrpSyscode = 0;
            try
            {
                SelectList grpSL = new SelectList(new List<SelectListItem>() { });

                DDLDTO grpDDLData = new DDLDTO(new List<DBTableNameEnums> { DBTableNameEnums.GroupMasterByEmp });
                grpDDLData.Predicate[DBTableNameEnums.GroupMasterByEmp]["GetData"] = true;
                grpDDLData.Predicate[DBTableNameEnums.GroupMasterByEmp].Add("employee_syscode", empSyscode);

                var response = new HttpResponseMessage();
                if (!client.DefaultRequestHeaders.Contains("Token"))
                    client.DefaultRequestHeaders.Add("Token", Convert.ToString(Session["Token"]));

                response = client.PostAsJsonAsync(cWebApiNames.APIGetDDLData, grpDDLData).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseMsg = response.Content.ReadAsStringAsync().Result;
                    grpDDLData = JsonConvert.DeserializeObject<DDLDTO>(responseMsg);
                    if (grpDDLData.opStatus)
                    {
                        List<SelectItemDTO> grpDDL = grpDDLData.Data.ExtractDDLDataForKey(DBTableNameEnums.GroupMasterByEmp);

                        if (grpDDL != null && grpDDL.Count > 0)
                        {
                            //Try to fetch the group syscode from the cookie.
                            HttpCookie StrideGroupCookie = HttpContext.Request.Cookies.Get("StrideGroup");
                            if (StrideGroupCookie != null)
                            {
                                if (!string.IsNullOrEmpty(StrideGroupCookie.Value))
                                {
                                    selectedGrpSyscode = Convert.ToInt32(StrideGroupCookie.Value);
                                }
                            }
                            if (ssGroupSyscode > 0)//Assign the group syscode from Session if cookie does not have value, if it is greater than 0.
                                selectedGrpSyscode = ssGroupSyscode;

                            if (selectedGrpSyscode == 0 || !grpDDL.Any(x=> Convert.ToInt32(x.Value) == selectedGrpSyscode))//If it is first login and session and cookie both are null then set default first value of the list.
                                int.TryParse(Convert.ToString(grpDDL.FirstOrDefault()?.Value), out selectedGrpSyscode);

                            grpSL = new SelectList(grpDDL, "Value", "Text", selectedGrpSyscode);                            
                        }
                    }
                }
                Session["GroupDDLSL"] = grpSL;
                Session["group_syscode"] = selectedGrpSyscode;
                AddGroupToCookie(selectedGrpSyscode);
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, ssLoggedInEmpName, ssLoggedInEmpSyscode.ToString(), "GetGroupDDLData", "BaseWebController");
            }
            return selectedGrpSyscode;
        }

        protected void AddGroupToCookie(int groupSyscode)
        {
            if (groupSyscode <= 0)
            {
                return;
            }
            HttpCookie StrideGroupCookie = new HttpCookie("StrideGroup");
            StrideGroupCookie.Value = groupSyscode.ToString();//ssGroupSyscode.ToString();
            StrideGroupCookie.HttpOnly = true;
            StrideGroupCookie.Expires = DateTime.Now.AddDays(5);
            Response.Cookies.Add(StrideGroupCookie);
        }
    }
}