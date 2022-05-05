using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Task_Tracker_CommonLibrary.Entity
{
    [System.ComponentModel.DataAnnotations.Schema.Table("project_master")]
    public class ProjectMaster :JMBaseBusinessEntity
    {

        [Key]
        public int project_syscode { get; set; }

        [DisplayName("Project")]
        public string project_name { get; set; }
        public IList<ModuleMaster> lstModules { get; set; }

        [DisplayName("Description")]
        public string project_description { get; set; }

        public int? group_syscode { get; set; }

        public IList<ProjectDetails> lstProjDetails { get; set; }

    }
}
