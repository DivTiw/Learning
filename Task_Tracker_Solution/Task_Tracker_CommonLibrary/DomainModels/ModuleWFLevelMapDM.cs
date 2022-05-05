using System;
using System.Collections.Generic;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    //[NotMapped] Not necessary as the entity is not inheriting from the POCO entity.
    public class ModuleWFLevelMapDM : JMBaseBusinessEntity
    {
        public DDLDTO ddlData = new DDLDTO(new List<DBTableNameEnums>() { DBTableNameEnums.ProjectMaster,
                                                                          DBTableNameEnums.ModuleMaster,
                                                                          DBTableNameEnums.GroupMember,
                                                                          DBTableNameEnums.CategoryMaster  });
        //public List<JMTask> lstTask { get; set; }

        public List<LevelTaskUserDTO> lstLevelTaskUsers { get; set; }

        public int project_syscode { get; set; }
        public int module_syscode { get; set; }

        //public string created_by_name { get; set; }

        public int category_syscode { get; set; }

    }

    ///ToDo: This class name is confusing don't confuse it with list of users.
    public class LevelTaskUserDTO 
    {
        public int level_syscode { get; set; }

        public int? task_syscode { get; set; }
        public string users { get; set; }
        public List<int> lstUsers { get; set; }
        public int[] arrUserSyscodes { get; set; }
        public string level_name { get; set; }

        public string workflow_name { get; set; }
        public decimal? weightage { get; set; }

        public string task_ref { get; set; }
        /// <summary>
        /// This is UI property utilized in the Project tasks page.
        /// This property is utilised for initiating the task after creating it from this level.
        /// </summary>
        public bool initiate { get; set; }
        public string level_details { get; set; }
        public DateTime? target_date { get; set; }
        public int? details_syscode { get; set; }

    }
}
