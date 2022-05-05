using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("release_details")]
    public class ReleaseDetails : JMBaseBusinessEntity
    {
        [Key]
        public int release_detail_syscode {get; set;}
        
        public int release_syscode { get; set; }

        public int parameter_syscode {get; set;}

        public string parameter_value {get; set;}


      


    }
}
