using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    public class LoginUser
    {
        public int employee_syscode { get; set; }
        public string employee_name { get; set; }
        public bool status { get; set; }
        public int? user_syscode { get; set; }
        public int? user_type_syscode { get; set; }
        public string user_role { get; set; }
        public string return_value { get; set; }
        public int department_syscode { get; set; }
        [NotMapped]
        public string token { get; set; }
    }
}
