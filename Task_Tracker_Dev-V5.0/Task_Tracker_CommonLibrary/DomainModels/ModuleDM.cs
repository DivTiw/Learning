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
    public class ModuleDM : ModuleMaster
    {
        public DDLDTO ddlData = new DDLDTO(new List<DBTableNameEnums>() { DBTableNameEnums.WorkflowMaster,
                                                                          DBTableNameEnums.GroupMember,
                                                                          DBTableNameEnums.CategoryMaster
                                                                        });

        public int[] arrReadUserSyscodes { get; set; }
        public int[] arrWriteUserSyscodes { get; set; }

        public string category_name { get; set; }
        public string project_name { get; set; }

        public int group_syscode { get; set; }

        //public IList<ModuleMaster> lstModules { get; set; }

        public bool blnProjectWriteAccess { get; set; }
    }
    
}
