using System.ComponentModel.DataAnnotations;

namespace Task_Tracker_CommonLibrary.Entity
{
    [System.ComponentModel.DataAnnotations.Schema.Table("workflow_level_details")]
    public class WorkflowLevelDetails : JMBaseBusinessEntity
    {
        [Key]
        public int level_syscode { get; set; }
        [MaxLength(100)]
        public string level_name { get; set; }
        public int level_order { get; set; }
        public int workflow_syscode { get; set; }
    }
}
