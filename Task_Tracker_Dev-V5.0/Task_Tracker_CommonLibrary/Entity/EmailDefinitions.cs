using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{

    [Table("email_definitions")]

    public class EmailDefinitions:JMBaseBusinessEntity
    {
        [Key]
        public int definition_syscode { get; set; }

        public int email_type_syscode { get; set; }

        public int? task_type_syscode { get; set; }

        public int? status_syscode { get; set; }

        public int template_syscode { get; set; }

        public string to_recipients { get; set; }

        public string cc_recipients { get; set; }
    }
}
