using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    [NotMapped]
    public class CategoryDM: CategoryMaster
    {
        //public DDLDTO ddlData = new DDLDTO(new List<DBTableNameEnums>() { DBTableNameEnums.vw_department_master });
    }
}
