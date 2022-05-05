using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.DomainModels;

namespace Task_Tracker_Solution.Areas.Master.Models
{
    public class ModuleWFLevelMapViewModel  : ModuleWFLevelMapDM
    {
        public ModuleWFLevelMapViewModel()
        {
            searchDTO = new SearchDTO();
        }
        public SelectList SLProjects { get; set; }
        public SelectList SLModules { get; set; }
        public MultiSelectList SLEmployee { get; set; }

        public SelectList SLCategory { get; set; }

        public SearchDTO searchDTO { get; set; }
    }

  
}