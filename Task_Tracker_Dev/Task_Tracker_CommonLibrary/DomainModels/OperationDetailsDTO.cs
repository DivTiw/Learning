using System;
using System.ComponentModel.DataAnnotations.Schema;
using Task_Tracker_CommonLibrary.Interface;

namespace Task_Tracker_CommonLibrary.DomainModels
{        
    public class OperationDetailsDTO : IOperationDetailsDTO
    {
        [NotMapped]
        public bool opStatus { get; set; }
        [NotMapped]
        public int opCode { get; set; }
        [NotMapped]
        public string opMsg { get; set; }
        [NotMapped]
        public string opUserFrndlyMsg { get; set; }
        [NotMapped]
        public Exception opInnerException { get; set; }

        [NotMapped]
        public bool PageHasWriteAccess { get; set; }
        [NotMapped]
        public bool RecordHasWriteAccess { get; set; }

        [NotMapped]
        public int logged_in_user { get; set; }

        [NotMapped]
        public string logged_in_user_name { get; set; }

    }
}
