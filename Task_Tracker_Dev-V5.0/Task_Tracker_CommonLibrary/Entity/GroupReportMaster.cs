using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("group_report_master")]
    public class GroupReportMaster : JMBaseBusinessEntity
    {
        [Key]
        public int grp_rpt_syscode { get; set; }
        public string report_name { get; set; }
        public int group_syscode { get; set; }
        public int template_syscode { get; set; }
        public string report_code { get; set; }
        public string report_desc { get; set; }
    }
}
