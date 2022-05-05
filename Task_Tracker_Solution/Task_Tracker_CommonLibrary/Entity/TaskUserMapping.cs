using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Task_Tracker_CommonLibrary.DomainModels;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("task_user_mapping")]
    public class TaskUserMapping : JMBaseBusinessEntity
    {
        [Key]
        public int user_mapping_syscode { get; set; }
        public int employee_syscode { get; set; }
        public int user_role_syscode { get; set; }
        public int task_syscode { get; set; }
        public int? trail_syscode { get; set; }

        ///ToDo: RED: Top Priority: Remove the search criteria dto from here in this table class
        /// and find a way to use it directly. REMOVE IT in first chance.
        [NotMapped]
        public SearchDTO searchCriteria { get; set; }

        //[MaxLength(200)]
        //public string type_detail { get; set; } //Ex. "Task" or "Trail"
        //public int type_syscode { get; set; } // Task_Syscode in case of Type_Detail as "Task"
    }
}
