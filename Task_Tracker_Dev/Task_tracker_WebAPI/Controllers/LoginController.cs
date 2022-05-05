//using Common_Components;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_Library.Repository;

namespace Task_tracker_WebAPI.Controllers
{
    public class LoginAPIController : ApiController
    {
        [HttpPost]
        public LoginUser AuthoriseUser([FromBody] LoginUser objLogin)
        {

            LoginUser obj = null;

            //LoginRepository lrepo = null; 

            try
            {
                if(string.IsNullOrEmpty(objLogin.employee_name))
                {
                    throw new Exception("Null Employee Name.");
                }

                using (var uow = new UnitOfWork())
                {
                    //lrepo = new LoginRepository();
                    obj = uow.LoginRepo.AuthoriseUser(objLogin.employee_name); 
                }
            }
            catch (Exception ex)
            {
                //Log.LogError(ex.Message, "", objLogin.employee_name + "", "AuthoriseUser", "LoginAPIController");
            }
            finally
            {
                //lrepo = null;
            }
            return obj;
        }

        [HttpPost]
        public DataSet GetAllMenu()
        {
            //LoginRepository lrepo = null;

            DataSet ds = new DataSet();

            try
            {
                using (var uow = new UnitOfWork())
                {
                    //lrepo = new LoginRepository();
                    ds = uow.LoginRepo.GetAllMenu(); 
                }
            }
            catch (Exception ex)
            {
                //Log.LogError(ex.Message, "", "" + "", "GetAllMenu", "LoginAPIController");
            }
            finally
            {
                //lrepo = null;
            }
           
            return ds;
        }

        [HttpPost]
        public DataSet GetWindowsLoginId([FromBody] int employee_syscode)
        {          
            DataSet ds = new DataSet();

            try
            {
                using (var uow = new UnitOfWork())
                {
                    ds = uow.LoginRepo.GetWindowsLoginId(employee_syscode);
                }              
            }
            catch (Exception ex)
            {
                
            }
         
            return ds;
        }

    }
}
