using System.ComponentModel.DataAnnotations;

namespace Task_Tracker_CommonLibrary.Entity
{
    [System.ComponentModel.DataAnnotations.Schema.Table("task_activity_master")]
    public class TaskActivityMaster
    {
        [Key]
        public int activity_syscode { get; set; }
        public string activity_name { get; set; }
        public string activity_icon { get; set; }
    }
}
