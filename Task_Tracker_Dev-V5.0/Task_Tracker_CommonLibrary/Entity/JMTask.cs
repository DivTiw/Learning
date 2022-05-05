using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Task_Tracker_CommonLibrary.Entity
{

    [System.ComponentModel.DataAnnotations.Schema.Table("task_master")]
    public class JMTask : JMBaseBusinessEntity
    {
        [Key]
        public int task_syscode { get; set; }
        public string task_details { get; set; }
        [DisplayName("Task ID")]
        public string task_reference { get; set; }
        [DisplayName("Subject")]
        public string task_subject { get; set; }
        public int task_status_syscode { get; set; }
        public int? task_priority_syscode { get; set; }
        public int? parent_task_syscode { get; set; }
        public int task_on_behalf { get; set; }
        public int task_owner { get; set; }

        [DisplayName("Target Date")]
        [DisplayFormat(DataFormatString = "{0:ddd, dd MMM yyyy}")]
        public DateTime? target_date { get; set; }
        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }
        public int? module_syscode { get; set; }
        public int department_syscode { get; set; }
        public int? category_syscode { get; set; }
        public int subcategory_syscode { get; set; }
        public int? level_syscode { get; set; }

        [DisplayName("Weightage")]
        public decimal weightage { get; set; }
        //public int? root_parent_task_syscode { get; set; }

        //public IList<TaskUserMapping> lstTaskUserMapping { get; set; }
    }
}
