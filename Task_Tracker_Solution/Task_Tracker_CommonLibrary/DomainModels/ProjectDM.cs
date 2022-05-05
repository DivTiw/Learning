using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    [NotMapped]
    public class ProjectDM : ProjectMaster
    {     
        public DDLDTO ddlData = new DDLDTO(new List<DBTableNameEnums>() { DBTableNameEnums.WorkflowMaster,
                                                                          DBTableNameEnums.GroupMember,      
                                                                          DBTableNameEnums.EnvironmentMaster,
                                                                          DBTableNameEnums.ProjectParameterMaster
                                                                        });

        public int[] arrReadUserSyscodes { get; set; }
        public int[] arrWriteUserSyscodes { get; set; }

        public int env_syscode { get; set; }    

    }
}


