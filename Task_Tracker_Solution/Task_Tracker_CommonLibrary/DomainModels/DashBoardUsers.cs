using System;
using System.Collections.Generic;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    public class DashBoard : TaskDM
    {
        
        public DashBoard()
        {
            lstWorkingUsers = new List<DashWorkingUsers>();
            lstUsersProjects = new List<DashUsersProjects>();
        }

        public List<DashWorkingUsers> lstWorkingUsers { get; set; }

        public List<DashUsersProjects> lstUsersProjects { get; set; }

        public DateTime? startdate { get; set; }

        public DateTime? enddate { get; set; }

        public int? ed_syscode { get; set; }

        public string JsonProjectUsersData { get; set; }
                
    }
}
