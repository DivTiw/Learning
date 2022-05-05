using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Task_Tracker_CommonLibrary.DomainModels;

namespace Task_Tracker_Solution.Areas.Master.Models
{
    public class ReleaseViewModel : ReleaseDM
    {
        public ReleaseViewModel()
        {
            //searchDTO = new SearchDTO();
        }
        public IList<ReleaseDM> lstReleaseDM { get; set; }
        //public SearchDTO searchDTO { get; set; }
    }
}