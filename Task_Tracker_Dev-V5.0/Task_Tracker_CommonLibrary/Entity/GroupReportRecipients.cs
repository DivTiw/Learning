using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("report_recipients")]
    public class GroupReportRecipients : JMBaseBusinessEntity
    {
        [Key]
        public int recipient_syscode { get; set; }
        public int employee_syscode { get; set; }
        public int grp_rpt_syscode { get; set; }
        public bool is_cc { get; set; }       
    }
}
