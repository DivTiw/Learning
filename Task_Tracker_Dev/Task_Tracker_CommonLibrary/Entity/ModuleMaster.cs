using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("module_master")]
    public class ModuleMaster : JMBaseBusinessEntity
    {
        [Key]
        public int module_syscode { get; set; }
        [DisplayName("Module")]
        [MaxLength(100)]
        public string module_name { get; set; }
        public int project_syscode { get; set; }        
        public int workflow_syscode { get; set; }

        [DisplayName("Description")]
        public string module_description { get; set; }

        public int? category_syscode { get; set; }

        //public IList<WorkflowLevelDetails> lstWorkflowLevels { get; set; }
    }
}
