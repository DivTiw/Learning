using System.ComponentModel.DataAnnotations;

namespace Task_Tracker_CommonLibrary.Entity
{
    [System.ComponentModel.DataAnnotations.Schema.Table("task_access_group_menu_details")]
    public class TaskAccessGroupMenuDetails
    {
        [Key]
        public int access_group_menu_details_syscode { get; set; }
        public int access_group_syscode { get; set; }        
        public int menu_syscode { get; set; }
    }
}
