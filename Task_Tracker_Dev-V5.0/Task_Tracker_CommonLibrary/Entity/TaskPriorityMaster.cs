using System.ComponentModel.DataAnnotations;

namespace Task_Tracker_CommonLibrary.Entity
{
    [System.ComponentModel.DataAnnotations.Schema.Table("task_priority_master")]
    public class TaskPriorityMaster
    {
        [Key]
        public int priority_syscode { get; set; }
        public string priority_name { get; set; }
        public string priority_icon { get; set; }
    }
}
