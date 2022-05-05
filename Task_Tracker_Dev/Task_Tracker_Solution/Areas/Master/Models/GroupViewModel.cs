using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Solution.Areas.Master.Models
{
    public class GroupViewModel : GroupDM
    {
        public MultiSelectList mslGrpHeads { get; set; }
        public MultiSelectList mslGrpMembers { get; set; }
        //public MultiSelectList mslEmployeeList { get; set; }
        public string json_mslEmployeeList { get; set; }
    }
}