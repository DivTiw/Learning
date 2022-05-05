using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("project_details")]
    public class ProjectDetails : JMBaseBusinessEntity
    {
        [Key]
        public int details_syscode { get; set; }
        
        public int env_syscode { get; set; }
        public int project_syscode { get; set; }

        public int parameter_syscode { get; set; }
        public string parameter_value { get; set; }


        [NotMapped]
        public string parameter_name { get; set; }

        [NotMapped]
        public bool editable_in_release { get; set; }
    }
}
