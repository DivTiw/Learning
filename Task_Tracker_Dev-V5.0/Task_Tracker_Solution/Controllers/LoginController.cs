using Common_Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Utility;

namespace Task_Tracker_Solution.Controllers
{
    public class LoginController : BaseWebController
    {
        TokenServer ts = null;
        public LoginController()
        {
            string TokenDbConnection = ConfigurationManager.ConnectionStrings["tokenAddress"].ToString();
            ts = new TokenServer(TokenDbConnection);
        }
        public ActionResult Index()
        {
            string mSuccMsg = string.Empty;
            string returnUrl = string.Empty;
            string decodetoken = "";
            string token = "";
            string windowsLoginid = "";

            try
            {
                if (Request.QueryString["token"] != null && !String.IsNullOrEmpty(Request.QueryString["token"]))
                {
                    token = Request.QueryString["token"].ToString();

                    if (ts.ValidateToken(token))
                    {

                        decodetoken = ts.DecodeToken(token);
                        if (!string.IsNullOrEmpty(decodetoken))
                        {
                            Dictionary<string, object> decodeValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(decodetoken);
                            if (decodeValues.Count > 0)
                            {
                                windowsLoginid = this.GetWindowsLoginId(Convert.ToInt32(decodeValues["emp_syscode"]));
                            }
                            if (AutoLogin(windowsLoginid))
                            {
                                if (decodeValues["returnVal"] != null)
                                {
                                    returnUrl = decodeValues["returnVal"].ToString();
                                }
                                else
                                {
                                    return RedirectToAction("GetAdminDashboard", "Dashboard", new { area = "Dashboard" });
                                }
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Either your Windows Login Id / Password field combination is wrong or you do not have access to this system.";
                                return RedirectToAction("Logout", "Login", new { Area = "", id = "Err", ErrMsg = TempData["ErrorMessage"].ToString() });
                            }
                            returnUrl = ConfigurationManager.AppSettings["SiteURL"] + returnUrl;
                            return Redirect(returnUrl);
                        }
                    }
                    else
                    {
                        string URL = ConfigurationManager.AppSettings["eLoginUrl"].ToString();
                        Response.Redirect(URL + "&returnValue=" + ConfigurationManager.AppSettings["TokenExpiryMsg"].ToString());
                    }
                }
                if (Request.QueryString["_l"] != null && !string.IsNullOrEmpty(Request.QueryString["_l"]))//For extreme case
                {
                    string windowsLoginId = ComLibCommon.Base64Decode(Convert.ToString(Request.QueryString["_l"]));
                    string[] LoginID = windowsLoginId.Split('|');
                    string sTime = LoginID[0];
                    string mLoginId = LoginID[1];


                    string mCurrentDT = DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss");
                    string mRequestTime = sTime;

                    TimeSpan tmpSpan = (DateTime.Parse(mCurrentDT) - DateTime.Parse(mRequestTime));

                    int mTimeSpan = Convert.ToInt32(ConfigurationManager.AppSettings["LoginTimeSpan"]);

                    if (tmpSpan.Minutes == 0 && tmpSpan.Seconds < mTimeSpan)
                    {
                        if (!string.IsNullOrEmpty(mLoginId))
                        {
                            AutoLogin(mLoginId);
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Invalid Action!!! URL expired.";
                        return RedirectToAction("Logout", new { Area = "" });
                    }
                }
                else
                {
                    string strName = User.Identity.Name.ToString();
                    AutoLogin(strName);
                }

                if (Convert.ToBoolean(this.Session["user_session"]))
                {
                    if (Request.QueryString["returnValue"] != null && !Request.QueryString["returnValue"].ToUpper().Contains("/LOGIN"))
                    {
                        returnUrl = Request.QueryString["returnValue"].ToString();
                    }
                    else
                    {
                        return RedirectToAction("GetAdminDashboard", "Dashboard", new { area = "Dashboard" });
                    }

                    return Redirect(returnUrl);
                }
                else
                {
                    TempData["ErrorMessage"] = "Either your Windows Login Id / Password field combination is wrong or you do not have access to this system.";
                    TempData["ErrorMessage"] = string.Empty;
                    return RedirectToAction("Logout", "Login", new { Area = "" });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "Index", "LoginController");
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }

        public ActionResult Logout(string id, string ErrMsg)
        {
            string mErrMsg = string.Empty;
            mErrMsg = ErrMsg;

            if (id == "Err" || id == "LogOff")
            {
                id = string.Empty;
            }


            if (!string.IsNullOrEmpty(id) && this.Session["user_session"] == null)
            {
                id = Convert.ToString(ConfigurationManager.AppSettings["SessionExpiredMsg"]);
                mErrMsg = string.Empty;
            }

            Session.Clear();
            Session.Abandon();
            string eLoginUrl = ConfigurationManager.AppSettings["eLoginUrl"].ToString();
            eLoginUrl = new StringBuilder(eLoginUrl).Append("&returnValue=").Append(id).Append("&ErrMsg=").Append(mErrMsg).ToString();

            return new RedirectResult(eLoginUrl);
        }

        public ActionResult _M(string _ln)
        {
            string mSuccMsg = string.Empty;
            LoginUser mAccessUserObj = null;

            String returnUrl = string.Empty;
            try
            {
                mAccessUserObj = new LoginUser();                
                string logonName = Dectoken(_ln).ToString();

                if (!string.IsNullOrEmpty(logonName))
                {                    
                    mAccessUserObj = AuthoriseUser(logonName);

                    if (mAccessUserObj != null && mAccessUserObj.status)
                    {
                        mAccessUserObj.token = _ln;
                        SetSessions(mAccessUserObj);
                        BuildMenu(mAccessUserObj.employee_syscode);


                        if (Request.QueryString["returnValue"] != null && !Request.QueryString["returnValue"].ToUpper().Contains("/LOGIN"))
                        {
                            returnUrl = Request.QueryString["returnValue"].ToString();

                        }
                        else
                        {
                            return RedirectToAction("GetAdminDashboard", "Dashboard", new { area = "Dashboard" });
                        }
                        returnUrl = ComLibCommon.Base64Encode(returnUrl);
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Either your Windows Login Id / Password field combination is wrong or you do not have access to this system.";
                        return RedirectToAction("Logout", "Login", new { Area = "" });
                    }
                }
                else
                {
                    ///ToDo: Create this page.
                    return RedirectToAction("Index", "Error", new { area = "" });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "ManualLogin", "LoginController");

                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
            finally
            {
                //client = null;
            }
        }

        public bool AutoLogin(string windowsLoginId)
        {
            LoginUser mAccessUserObj = null;
            string logonName = string.Empty;
            bool mSuccMsg = false; //string.Empty;

            if (string.IsNullOrEmpty(windowsLoginId))
            {
                string strName = string.Empty;

                System.Security.Principal.WindowsPrincipal p = System.Threading.Thread.CurrentPrincipal as System.Security.Principal.WindowsPrincipal;
                if (p != null)
                {
                    strName = p.Identity.Name;
                }

                if (string.IsNullOrEmpty(strName))
                {
                    strName = User.Identity.Name;
                    if (string.IsNullOrEmpty(strName))
                    {
                        strName = Request.ServerVariables["AUTH_USER"]; //Finding with name
                    }
                }

                logonName = strName;
            }
            else
            {
                logonName = windowsLoginId;
            }


            //[START]
            if (!string.IsNullOrEmpty(logonName))
            {

                mAccessUserObj = AuthoriseUser(logonName);
            }
            //[END]

            if (mAccessUserObj != null && mAccessUserObj.status)
            {
                SetSessions(mAccessUserObj);
                BuildMenu(mAccessUserObj.employee_syscode);
                mSuccMsg = true;
                //mSuccMsg = mAccessUserObj.employee_name;
            }

            return mSuccMsg;
        }


        protected LoginUser AuthoriseUser(string logonName)
        {
            LoginUser mAccessUserObj = null;
            var response = new HttpResponseMessage();
            LoginUser user = new LoginUser();
            user.employee_name = logonName;
            response = client.PostAsJsonAsync("API/LoginAPI/AuthoriseUser", user).Result;
            if (response.IsSuccessStatusCode)
            {
                var clientResponse = response.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                mAccessUserObj = JsonConvert.DeserializeObject<LoginUser>(clientResponse, settings);
            }
            return mAccessUserObj;
        }


        public void SetSessions(LoginUser mAccessUserObj)
        {
            Session["user_session"] = true;
            Session["emp_name"] = mAccessUserObj.employee_name;
            Session["emp_syscode"] = mAccessUserObj.employee_syscode;
            Session["department_syscode"] = mAccessUserObj.department_syscode;
            Session["user_syscode"] = mAccessUserObj.user_syscode;
            Session["user_type_syscode"] = mAccessUserObj.user_type_syscode;
            TokenModel tm = new TokenModel();
            tm.UserID = mAccessUserObj.employee_syscode;
            tm.UserName = mAccessUserObj.employee_name;
            Session["Token"] = ts.GetToken(tm);//mAccessUserObj.token;
            SetGroupDDLData(mAccessUserObj.employee_syscode);
        }

        public IList<Menu> BuildMenu(int userSyscode)
        {
            IList<Menu> mmList = new List<Menu>();
            try
            {
                DataSet ds = new DataSet();

                if (HttpRuntime.Cache.Get("MenuData1") == null || HttpRuntime.Cache.Get("MenuData2") == null)
                {

                    var response = new HttpResponseMessage();
                    response = client.PostAsJsonAsync("api/LoginAPI/GetAllMenu", "").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var clientResponse = response.Content.ReadAsStringAsync().Result;
                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore
                        };
                        ds = JsonConvert.DeserializeObject<DataSet>(clientResponse, settings);
                    }


                    string _menuobj = "";
                    int i = 1;
                    foreach (DataTable dt in ds.Tables)
                    {

                        _menuobj = "MenuData" + i.ToString();
                        List<Menu> menus = new List<Menu>();

                        foreach (DataRow dr in dt.Rows)
                        {
                            Menu menu = new Menu();
                            menu.menu_syscode = Convert.ToInt32(dr["menu_syscode"].ToString());
                            menu.menu_name = Convert.ToString(dr["menu_name"]);
                            menu.menu_description = Convert.ToString(dr["menu_description"]);
                            menu.parent_menu_syscode = String.IsNullOrEmpty(dr["parent_menu_syscode"].ToString()) ? (int?)null : Convert.ToInt32(dr["parent_menu_syscode"].ToString());
                            menu.page_url = Convert.ToString(dr["page_url"]);
                            menu.icon = Convert.ToString(dr["icon"]);
                            menus.Add(menu);
                        }
                        HttpRuntime.Cache.Insert(_menuobj, menus, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
                        i += 1;
                    }
                }
                else
                {

                    //CommonLibrary.log.LogErrorToFile("Ander Nahi gaya hai cache active nahi hai:");
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, ssLoggedInEmpName,ssLoggedInEmpSyscode.ToString(), "BuildMenu", "LoginController");
            }
            return mmList;
        }

        public string GetWindowsLoginId(int employee_syscode)
        {
            string val = "";
            DataSet ds = new DataSet();
            var response = new HttpResponseMessage();
            response = client.PostAsJsonAsync("api/LoginAPI/GetWindowsLoginId", employee_syscode).Result;
            if (response.IsSuccessStatusCode)
            {
                var clientResponse = response.Content.ReadAsStringAsync().Result;
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };

                ds = JsonConvert.DeserializeObject<DataSet>(clientResponse, settings);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    val = Convert.ToString(ds.Tables[0].Rows[0][0]);
                }
            }
            return val;
        }

        public string Dectoken(string token)
        {
            string decToken = "", windowsLoginid = "", Gen_time = "";            
            if (ts.ValidateToken(token))
            {

                decToken = ts.DecodeToken(token);

                if (!string.IsNullOrEmpty(decToken))
                {
                    Dictionary<string, object> decodeValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(decToken);
                    if (decodeValues.Count > 0)
                    {
                        windowsLoginid = decodeValues["windows_login_id"].ToString();
                        Gen_time = decodeValues["Gen_time"].ToString();
                    }

                }
                string mCurrentDT = DateTime.Now.ToString("dd-MMM-yyy HH:mm:ss");
                string mRequestTime = Gen_time;

                TimeSpan tmpSpan = (DateTime.Parse(mCurrentDT) - DateTime.Parse(mRequestTime));

                int mTimeSpan;
                string timePath = Server.MapPath(ConfigurationManager.AppSettings["TimePath"]);
                using (StreamReader sr = new StreamReader(timePath))
                {
                    mTimeSpan = Convert.ToInt32(sr.ReadLine());
                }


                if (tmpSpan.Minutes == 0 && tmpSpan.Seconds < mTimeSpan)
                {
                    // code
                }
                else
                {
                    windowsLoginid = "";
                }
            }
            return windowsLoginid;

        }

    }
}