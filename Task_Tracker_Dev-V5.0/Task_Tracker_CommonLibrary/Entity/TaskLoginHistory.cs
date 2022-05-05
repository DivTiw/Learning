using System;
using System.ComponentModel.DataAnnotations;

namespace Task_Tracker_CommonLibrary.Entity
{
    [System.ComponentModel.DataAnnotations.Schema.Table("task_login_history")]
    public class TaskLoginHistory
    {
        [Key]
        public int login_history_syscode { get; set; }
        public int employee_syscode { get; set; }
        public DateTime login_on { get; set; }
        public int switched_user_syscode { get; set; }
    }
}
