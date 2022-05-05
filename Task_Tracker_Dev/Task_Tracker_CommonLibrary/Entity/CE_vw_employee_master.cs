using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("vw_employee_master")]
    public class CE_vw_employee_master : vw_employee_master
    {
        [NotMapped]
        public override int? employment_type_syscode { get; set; }
        [NotMapped]
        public override int? insurance_type_syscode { get; set; }
        [NotMapped]
        public override int? confirmation_ed_syscode { get; set; }
        [NotMapped]
        public override int? confirmation_coed_syscode { get; set; }

        [NotMapped]
        public override bool? is_autoconfirmed { get; set; }

        [NotMapped]
        public override string sub_department_name { get; set; }
    }
}
