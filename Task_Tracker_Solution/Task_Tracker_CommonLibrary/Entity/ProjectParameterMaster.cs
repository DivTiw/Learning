using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("project_parameter_master")]
    public class ProjectParameterMaster : JMBaseBusinessEntity
    {

        [Key]
        public int parameter_syscode { get; set; }
        public int group_syscode { get; set; }
        public string parameter_name { get; set; }
        public string parameter_desc { get; set; }

        public bool editable_in_release { get; set; }

        public string parameter_datatype { get; set; }

    }
}
