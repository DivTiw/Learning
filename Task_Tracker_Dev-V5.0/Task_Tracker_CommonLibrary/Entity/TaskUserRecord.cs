using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.DomainModels;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("task_user_record")]
    public class TaskUserRecord : OperationDetailsDTO
    {
        [Key]
        public int record_syscode { get; set; }

        public int employee_syscode { get; set; }

        public int task_syscode { get; set; }

        [DisplayFormat(DataFormatString = "{0:ddd, dd MMM yyyy HH:mm}")]
        public DateTime? start_time { get; set; }


        [DisplayFormat(DataFormatString = "{0:ddd, dd MMM yyyy HH:mm}")]
        public DateTime? stop_time { get; set; }

        [NotMapped]
        public string duration { get; set; }

    }
}
