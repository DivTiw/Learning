using System.ComponentModel.DataAnnotations;
namespace Task_Tracker_CommonLibrary.Entity
{
    [System.ComponentModel.DataAnnotations.Schema.Table("task_user_role_master")]
    public class TaskUserRoleMaster
    {
        [Key]
        public int role_syscode { get; set; }
        [MaxLength(100)]
        public string role_name { get; set; }

        public string role_code { get; set; }
    }
}
