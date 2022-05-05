
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Task_Tracker_CommonLibrary.Entity
{
    [System.ComponentModel.DataAnnotations.Schema.Table("task_status_master")]
    public class TaskStatusMaster
    {
        [Key]
        public int status_syscode { get; set; }
        [DisplayName("Status")]
        public string status_name { get; set; }
        public string status_icon { get; set; }
    }
}
