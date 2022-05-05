using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("task_trail")]
    public class TaskTrail :JMBaseBusinessEntity
    {
        [Key]
        public int trail_syscode { get; set; }
        public int task_syscode { get; set; }
        public int activity_syscode { get; set; }
        public DateTime? trail_start_datetime { get; set; }
       
        [MaxLength(1000)]
        public string trail_description { get; set; }
        [MaxLength(2000)]
        public string trail_comments { get; set; }
               
        [NotMapped]
        public override bool is_active { get; set; }
        public TaskActivityMaster Activity { get; set; }

        public int trail_group_id { get; set; }
        public string inform_to { get; set; }
    }
}
