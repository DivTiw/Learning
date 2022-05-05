using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("proj_mod_user_mapping")]
    public class ProjModUserMapping : JMBaseBusinessEntity
    {
        [Key]
        public int mapping_syscode { get; set; }
        public int employee_syscode { get; set; }
        public int role_syscode { get; set; }
        public int project_syscode { get; set; }
        public int? module_syscode { get; set; }
        public bool access_read { get; set; }
        public bool access_write { get; set; }
    }
}
