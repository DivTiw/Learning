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
using Task_Tracker_Solution.Areas.Tasks.Models;
using Task_Tracker_Solution.Utility;

namespace Task_Tracker_Solution.Areas.Master.Controllers
{
    public class CategoryController : TaskTrackerBaseController
    {
        // GET: Master/Category
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateCategory(string id = null)
        {
            //TaskViewModel taskViewModel = null;
            CategoryViewModel Catmodel = null;
            try
            {
                string sreturn = string.Empty;
                if (this.Session["emp_syscode"] != null)
                {
                    int loggedin_user = Convert.ToInt32(Convert.ToString(this.Session["emp_syscode"]));
                    int cat_syscode = String.IsNullOrEmpty(id) ? 0 : Convert.ToInt32(ComLibCommon.Base64Decode(id));

                    Catmodel = new CategoryViewModel();
                    Catmodel.category_syscode = cat_syscode;
                    Catmodel.group_syscode = ssGroupSyscode;
                   
                    //Catmodel.ddlData.Predicate[DBTableNameEnums.vw_department_master]["GetData"] = true;                 

                    if (cat_syscode > 0)
                    { 
                        HttpResponseMessage response = client.PostAsJsonAsync(cWebApiNames.APIGetCategoryByID, Catmodel).Result;


                    var responseMsg = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {
                        //if (cat_syscode > 0)
                        //    Catmodel = JsonConvert.DeserializeObject<CategoryViewModel>(responseMsg);
                        //else
                        //    Catmodel.ddlData = JsonConvert.DeserializeObject<DDLDTO>(responseMsg);

                        Catmodel = JsonConvert.DeserializeObject<CategoryViewModel>(responseMsg);
                            Catmodel.category_syscode = cat_syscode;
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

            return View(Catmodel);
        }

        public ActionResult CategoryList()
        {
            IList<CategoryViewModel> Category = null;
            try
            {
                string sreturn = string.Empty;
                if (this.Session["emp_syscode"] != null)
                {                    
                    CategoryViewModel Catmodel = new CategoryViewModel();
                    Catmodel.group_syscode = ssGroupSyscode;

                    var response = client.PostAsJsonAsync(cWebApiNames.APIGetAllCategoryList, Catmodel).Result;
                    var responseMsg = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode)
                    {

                        Category = JsonConvert.DeserializeObject<IList<CategoryViewModel>>(responseMsg);
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

            return View("CategoryList", Category);

        }

        [HttpPost]
        public async Task<ActionResult> Submit(CategoryViewModel Catmodel, string action)
        {
            try
            {
                string sreturn = string.Empty;
                if (this.Session["emp_syscode"] != null)
                {
                    var response = new HttpResponseMessage();
                    int emp_syscode = int.Parse(Session["emp_syscode"].ToString());

                    Catmodel.created_by = emp_syscode;
                    Catmodel.group_syscode = ssGroupSyscode;

                    #region "Save category"
                    if (action == "save_category")
                    {
                        if (Catmodel.group_syscode <= 0)
                        {
                            throw new Exception("Please select Group.");
                        }

                        if (string.IsNullOrEmpty(Catmodel.category_name))
                        {
                            throw new Exception("Please enter Category Name");
                        }

                        if (Catmodel.category_syscode > 0)
                        {
                            response = await client.PutAsJsonAsync(Utility.cWebApiNames.APIUpdateCategory, Catmodel);
                        }
                        else
                        {
                            response = await client.PostAsJsonAsync(Utility.cWebApiNames.APIPostCategory, Catmodel);
                        }
                        if (response.IsSuccessStatusCode)
                        {
                            var Response = response.Content.ReadAsStringAsync().Result;
                            Catmodel = JsonConvert.DeserializeObject<CategoryViewModel>(Response);

                            if (Catmodel.opStatus)
                            {                              
                                ViewBag.SuccessMessage = "Record saved sucessussfully.";
                                return RedirectToAction("CategoryList");
                            }
                            else
                            {
                                throw new Exception(Catmodel.opMsg, Catmodel.opInnerException);
                            }
                        }
                        else
                        {
                            throw new Exception(response.ReasonPhrase);
                        }
                        //----------
                        //Catmodel.ddlData.Predicate[DBTableNameEnums.vw_department_master]["GetData"] = true;
                        //  HttpResponseMessage response;
                        //response = client.PostAsJsonAsync(cWebApiNames.APIGetDDLData, Catmodel.ddlData).Result;
                        //var responseMsg = response.Content.ReadAsStringAsync().Result;
                        //if (response.IsSuccessStatusCode)
                        //{
                        //    Catmodel.ddlData = JsonConvert.DeserializeObject<DDLDTO>(responseMsg);
                        //    if (Catmodel.ddlData.opStatus)
                        //    {
                        //        Catmodel.SLdepartment = new SelectList(Catmodel.ddlData.Data.ExtractDDLDataForKey(DBTableNameEnums.vw_department_master), "Value", "Text", 0);
                        //    }
                        //    // Catmodel.category_syscode = cat_syscode;
                        //}
                        //----------------

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
                return View("CreateCategory", Catmodel);
            }

            return View("CreateCategory", Catmodel);
        }

    }
}