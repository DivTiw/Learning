using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("module_level_user_mapping")]
    public class ModuleLevelUserMapping : JMBaseBusinessEntity
    {

        [Key]
        public int usrmap_syscode { get; set;}
        public int details_syscode { get; set; }
        public int employee_syscode { get; set; }
    }


}
