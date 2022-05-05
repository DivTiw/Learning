using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    [NotMapped]
    public class ReleaseDM : ReleaseInstructions
    {
        public ReleaseDM()
        {
            task = new JMTask();
            project = new ProjectMaster();
        }
        public JMTask task { get; set; }

        public ProjectMaster project { get; set; }

        public EnvironmentMaster env { get; set; }

      
        //public string created_by_name { get; set; }

        //public string tbl_htmlstring { get; set; }
        public string release_params_json { get; set; }

        public IList<ReleaseDetailsDM> lstReleaseDetailsDM { get; set; }

    }
}
