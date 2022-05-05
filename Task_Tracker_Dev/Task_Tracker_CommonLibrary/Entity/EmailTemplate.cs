using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("email_template")]
    public class EmailTemplate : JMBaseBusinessEntity
    {
        [Key]
        public int template_syscode { get; set; }

        public string template_name { get; set; }
        public string template_subject { get; set; }
        public string template_body { get; set; }
        public string from_email_display { get; set; }
        public string from_email_id { get; set; }
        public string template_header { get; set; }
        public string template_footer { get; set; }
        public string link_url { get; set; }        

        [NotMapped]
        public override bool is_deleted { get; set; }
    }
}
