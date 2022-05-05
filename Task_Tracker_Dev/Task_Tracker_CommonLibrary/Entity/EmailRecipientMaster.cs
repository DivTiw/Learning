using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{

    [Table("email_recipient_master")]
    public class EmailRecipientMaster
    {

        [Key]
        public int email_recipient_syscode { get; set; }

        public string email_recipient_name { get; set; }

        public string description { get; set; }

        public string code { get; set; }
    }
}
