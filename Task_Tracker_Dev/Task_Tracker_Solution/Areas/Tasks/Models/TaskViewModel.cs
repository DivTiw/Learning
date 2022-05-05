using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.DomainModels;

namespace Task_Tracker_Solution.Areas.Tasks.Models
{
    public class TaskViewModel : TaskDM
    {
        public TaskViewModel()
        {
            searchDTO = new SearchDTO();
        }
        public SelectList SLPriority { get; set; }
        public SelectList SLStatus { get; set; }
        public SelectList SLUsers { get; set; } = new SelectList(new List<SelectListItem>() { });
        public MultiSelectList MSLEmployees { get; set; }
        public SelectList SLCategory{ get; set; }

        public SelectList SLdepartment { get; set; }

        public List<TaskDM> lstTaskDM { get; set; }
        public SearchDTO searchDTO { get; set; }

        public bool isProjectTaskPage { get; set; }

        
    }
}