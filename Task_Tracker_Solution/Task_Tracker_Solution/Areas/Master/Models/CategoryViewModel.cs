using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Solution.Areas.Master.Models
{
    public class CategoryViewModel : CategoryDM
    {
       // TaskDM dm = new TaskDM();
        public int row_Id { get; set; }
        //public SelectList SLdepartment { get; set; }
        //public string department_name { get; set; }

    }
}