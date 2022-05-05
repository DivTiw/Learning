using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("workflow_master")]
    public class WorkflowMaster : JMBaseBusinessEntity
    {
        [Key]
        [DDLValue]
        public int workflow_syscode { get; set; }

        [MaxLength(200)]
        [DDLText]
        public string workflow_name { get; set; }

        [MaxLength(100)]
        public string dept_name { get; set; }
        public IList<ModuleMaster> lstModules { get; set; }

        public IList<WorkflowLevelDetails> lstWFLevels{ get; set; }

        public int? group_syscode { get; set; }
    }
}
