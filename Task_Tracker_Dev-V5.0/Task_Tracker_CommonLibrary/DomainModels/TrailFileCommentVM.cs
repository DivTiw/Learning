using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    public class TrailFileCommentVM
    {
        public string TrailDesc { get; set; }
        public string TrailFiles { get; set; }
        public string TrailComment { get; set; }

        public bool isDuplicate { get; set; }
        //public int groupid { get; set; }
    }
}
