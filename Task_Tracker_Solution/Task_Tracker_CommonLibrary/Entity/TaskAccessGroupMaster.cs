using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("task_access_group_master")]
    public class TaskAccessGroupMaster : JMBaseBusinessEntity
    {
        [Key]
        public int access_group_syscode { get; set; }
        [MaxLength(100)]
        public string access_group_name { get; set; }
        [MaxLength(100)]
        public string remark { get; set; }
        [MaxLength(10)]
        public string group_type { get; set; }
        public int group_syscode { get; set; }        
    }
}
