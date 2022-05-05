using System.ComponentModel.DataAnnotations;

namespace Task_Tracker_CommonLibrary.Entity
{
    [System.ComponentModel.DataAnnotations.Schema.Table("task_category_contact")]
    public class TaskCategoryContact : JMBaseBusinessEntity
    {
        [Key]
        public int category_contact_syscode { get; set; }
        public int category_syscode { get; set; }
        public string email_id { get; set; }
    }
}
