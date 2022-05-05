using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("email_type_master")]
    public class EmailTypeMaster
    {
        [Key]
        public int email_type_syscode { get; set; }

        public string email_type_name { get; set; }

        public string description { get; set; }

        public string code { get; set; }
    }
}
