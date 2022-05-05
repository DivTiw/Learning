using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    /// <summary>
    /// This is DTO for [proc_get_GroupMember_TaskDtls]
    /// </summary>
    public class GroupMember_TaskDtlsDTO
    {
        public string encrypted_task_syscode { get; set; }
        public int employee_syscode { get; set; }

        public int? task_syscode { get; set; }
        public string task_reference { get; set; }
        public string task_subject { get; set; }
        public string status_name { get; set; }
        public int? task_owner { get; set; }
        public int? task_on_behalf { get; set; }
        public int? created_by { get; set; }
        public string module_name { get; set; }
        public string project_name { get; set; }
        public int? group_syscode { get; set; }
        public decimal? Total_Hours_Worked { get; set; }
        public decimal? Total_Days_Worked { get; set; }
        public string Parent_Subject { get; set; }
        public string employee_name { get; set; }
    }
}
