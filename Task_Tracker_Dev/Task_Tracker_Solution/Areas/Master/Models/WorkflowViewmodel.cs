using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Solution.Areas.Master.Models
{
    public class WorkflowViewmodel : WorkflowDM
    {
        public int row_Id { get; set; }
        public string lstWFLevelsJson { get; set; }
    }
}