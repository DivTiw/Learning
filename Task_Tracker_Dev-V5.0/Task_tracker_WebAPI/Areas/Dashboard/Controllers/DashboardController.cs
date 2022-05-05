using Common_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_Library.Repository;

namespace Task_tracker_WebAPI.Areas.Dashboard.Controllers
{
    public class DashboardController : ApiController
    {
        [HttpPost]
        public List<TaskDM> GetMyTaskActivity(TaskUserMapping taskUser)
        {
            List<TaskDM> taskDM = null;
            try
            {
                using (var uow = new UnitOfWork())
                {
                    taskDM = new List<TaskDM>();
                    taskDM = uow.DashboardRepo.getTodaysLatestTasks(taskUser.employee_syscode, true);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", taskUser.employee_syscode.ToString(), "GetMyTaskActivity", "DashboardAPIController");
            }
            return taskDM;
        }

        [HttpPost]
        public List<TaskDM> GetTodaysLatestTasks(TaskUserMapping taskUser)
        {
            List<TaskDM> taskDM = null;
            try
            {
                using (var uow = new UnitOfWork())
                {
                    taskDM = new List<TaskDM>();
                    taskDM = uow.DashboardRepo.getTodaysLatestTasks(taskUser.employee_syscode);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", taskUser.employee_syscode.ToString(), "OpenTasks", "DashboardAPIController");
            }
            return taskDM;
        }

        [HttpPost]
        public DashBoard GetTeamActivity(DashBoard dashInput)
        {
            DashBoard dashboard = null;
            try
            {
                using (var uow = new UnitOfWork())
                {
                    dashboard = new DashBoard();
                    dashboard = uow.DashboardRepo.getTeamActivity(dashInput);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", dashInput.ed_syscode.Value.ToString(), "GetTeamActivity", "DashboardAPIController");
            }
            return dashboard;
        }

        [HttpPost]
        public DashBoard GetDashboard(DashBoard taskUser)
        {
            DashBoard dashboard = null;
            try
            {
                using (var uow = new UnitOfWork())
                {
                    dashboard = new DashBoard();
                    dashboard = uow.DashboardRepo.getDashboard(taskUser);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", taskUser.ed_syscode.Value.ToString(), "GetWorkingUsers", "DashboardAPIController");
            }
            return dashboard;
        }             
    }
}
