using System.Collections.Generic;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Solution.Areas.Master.Models
{
    public class ModuleViewModel : ModuleDM
    {
        public List<SelectItemDTO> SLWorkflowList { get; set; }

        public List<SelectItemDTO> SLCategory { get; set; }
        public List<SelectItemDTO> SLGroupMembers { get; set; }

        public string CategoryListJson { get; set; }
        public string WorkflowListJson { get; set; }
        public string MemberListJson { get; set; }

        public List<ModuleDM> lstModuleDM { get; set; }
    }
}