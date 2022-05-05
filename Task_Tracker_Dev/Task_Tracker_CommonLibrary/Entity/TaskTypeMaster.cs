using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("task_type_master")]

    public class TaskTypeMaster
    {
        [Key]
        public int task_type_syscode { get; set; }

        public string task_type { get; set; }

        public string description { get; set; }

        public string code { get; set; }
    }
}
