using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    public abstract class DashUsers
    {
        public string emp_name { get; set; }

        public string ed_name { get; set; }
        public int? group_syscode { get; set; }
        public string project_name { get; set; }
        public string module_name { get; set; }

    }
}
