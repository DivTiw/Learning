using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    public class SearchDTO
    {
        public DDLDTO ddlData = new DDLDTO(new List<DBTableNameEnums>() { DBTableNameEnums.ProjectMaster,
                                                                          DBTableNameEnums.ModuleMaster,                                                                         
                                                                          DBTableNameEnums.CategoryMaster,
                                                                           DBTableNameEnums.Tasks,
                                                                           DBTableNameEnums.TaskStatusMaster});
        public string actionName { get; set; }
        /// <summary>
        /// This holds the plain text passed from the search text bar in the search controllers
        /// </summary>
        public string txtSearch { get; set; }

        public SelectList SLProjects { get; set; }
        public SelectList SLModules { get; set; }
        public SelectList SLCategory { get; set; }

        public int? group_syscode { get; set; }
        public int project_syscode { get; set; }
        public int module_syscode { get; set; }
        public int category_syscode { get; set; }
        public int employee_syscode { get; set; }

        /*UI related properties*/
        public bool enableSearchTextBox { get; set; } = true;
        public bool enableTaskDD { get; set; } = true;

        public bool enableStatusDD { get; set; } = true;

        public int task_syscode { get; set; }

        public SelectList SLTasks { get; set; }

        public SelectList SLStatus { get; set; }

        public int status_syscode { get; set; }

        public bool enableCategoryDD { get; set; } = true;

        public bool enableModuleDD { get; set; } = true;
    }
}