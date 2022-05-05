using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    public class DashUsersProjects : DashUsers
    {
        #region DashboardProjects
        public int? hours_spent { get; set; }
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }

        public int? days_spent { get; set; }
        #endregion

    }
}
