using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Solution.Areas.Master.Models
{
    public class ProjectViewModel : ProjectDM
    {

        public ProjectViewModel()
        {
            searchDTO = new SearchDTO();
        }
        //public string ModulesListJson { get; set; }
        //public string WorkflowListJson { get; set; }
        //public List<ModuleViewModel> lstModuleVM { get; set; }
        public List<SelectItemDTO> SLWorkFlow { get; set; }
        public List<SelectItemDTO> SLGroupMembers { get; set; }

        public string MemberListJson { get; set; }

        public SelectList SLEnvironment { get; set; }

        public string lstProjDetailsJson { get; set; }

        public List<SelectItemDTO> lstParameter { get; set; }

        public SearchDTO searchDTO { get; set; }

        public IList<ProjectDM> lstProjDM { get; set; }
    }
}