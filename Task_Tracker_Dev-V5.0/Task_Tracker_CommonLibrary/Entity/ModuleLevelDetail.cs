using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("module_level_detail")]
    public class ModuleLevelDetail : JMBaseBusinessEntity
    {
        [Key]
        public int details_syscode { get; set; }
        public int level_syscode { get; set; }     
        public int module_syscode { get; set; }
        public decimal weightage { get; set; }
    }
}
