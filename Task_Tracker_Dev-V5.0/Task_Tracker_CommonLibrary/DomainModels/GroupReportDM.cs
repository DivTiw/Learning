using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    [NotMapped]
    public class GroupReportDM : GroupReportMaster
    {
        public string toEmails { get; set; }
        public string toNames { get; set; }
        public string ccEmails { get; set; }

        public IList<GroupReportRecipientDM> lstReportRecipients { get; set; }
    }
    
}
