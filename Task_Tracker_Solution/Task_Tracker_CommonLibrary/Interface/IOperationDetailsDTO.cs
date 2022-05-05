using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Tracker_CommonLibrary.Interface
{
    public interface IOperationDetailsDTO
    {
        [NotMapped]
        bool opStatus { get; set; }
        [NotMapped]
        int opCode { get; set; }
        [NotMapped]
        string opMsg { get; set; }
        [NotMapped]
        string opUserFrndlyMsg { get; set; }
        [NotMapped]
        Exception opInnerException { get; set; }
    }
}
