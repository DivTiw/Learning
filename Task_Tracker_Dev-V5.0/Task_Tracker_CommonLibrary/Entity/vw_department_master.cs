using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("vw_department_master")]
    public class vw_department_master
    {
        [Key]
        public int department_syscode { get; set; }
        public string department_name { get; set; }
        public int? created_by { get; set; }
        public DateTime? created_on { get; set; }
        public int? last_updated_by { get; set; }
        public DateTime? last_updated_date { get; set; }
    }
}
