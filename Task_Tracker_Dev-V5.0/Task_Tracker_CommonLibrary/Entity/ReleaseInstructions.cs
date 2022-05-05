using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("release_instructions")]
    public class ReleaseInstructions : JMBaseBusinessEntity
    {
        [Key]
        public int release_syscode { get; set; }
        public string release_ref { get; set; }      
	    public int env_syscode { get; set; }
        public int project_syscode { get; set; }
        public int task_syscode { get; set; }
        public bool is_Released { get; set; } = false;
        public string Remarks { get; set; }

        [NotMapped]
        public IList<ReleaseDetails> lstReleaseDetails { get; set; }
    }
}
