using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("environment_master")]
    public class EnvironmentMaster : JMBaseBusinessEntity
    {
        [Key]
        public int env_syscode { get; set; }
        public int group_syscode  { get; set; }
        public string env_name { get; set; }
        public string env_code  { get; set; }
        public string env_desc { get; set; }
    }
}
