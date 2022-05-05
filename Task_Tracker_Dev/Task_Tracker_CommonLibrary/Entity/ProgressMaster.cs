using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("progress_master")]
    public class ProgressMaster : JMBaseBusinessEntity
    {
        [Key]
        public int progress_syscode { get; set; }
        public string type_detail { get; set; } //('Project' OR 'Module' OR 'Task')
        public int type_syscode { get; set; }
        public decimal progress { get; set; }
        [NotMapped]
        public override bool is_active { get; set; }
    }
}
