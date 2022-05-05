using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    [NotMapped]
    public class GroupReportRecipientDM : GroupReportRecipients
    {
        [NotMapped]
        public string Recipient_Name { get; set; }
        [NotMapped]
        public string Recipient_Email { get; set; }
    }
    
}
