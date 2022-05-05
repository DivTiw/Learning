using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    public class DashWorkingUsers : DashUsers
    {
        #region Working Users
        public bool isWorking { get; set; }
        public int? task_syscode { get; set; }

        public string task_subject { get; set; }

        public DateTime? start_time { get; set; }
        public DateTime? stop_time { get; set; }
        public string duration { get; set; }

        public string parent_task_subject { get; set; }
        #endregion
    }
}
