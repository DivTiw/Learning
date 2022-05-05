using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    [NotMapped]
    public class GroupDM : GroupMaster
    {
        public DDLDTO ddlData = new DDLDTO(new List<DBTableNameEnums>() { DBTableNameEnums.vw_employee_master});
        public int[] arrGrpHeadSyscodes { get; set; } = { };
        public int[] arrMemSyscodes { get; set; } = { };

        //public string created_by_name { get; set; }
    }
}
