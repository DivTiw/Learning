using System.ComponentModel.DataAnnotations;

namespace Task_Tracker_CommonLibrary.Entity
{
    [System.ComponentModel.DataAnnotations.Schema.Table("task_trail_details")]
    public class TaskTrailDetails
    {
        [Key]
        public int trail_details_syscode { get; set; }
        public int trail_syscode { get; set; }
        [MaxLength(1000)]
        public string trail_description { get; set; }
        [MaxLength(1000)]
        public string trail_comments { get; set; }
    }
}
