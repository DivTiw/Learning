using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Library.Repository
{
    public class ProjectRepository : TTBaseRepository<ProjectMaster>
    {
        public ProjectRepository(TTDBContext masterContext) : base(masterContext)
        {
        }

        public List<ProjectDetails> GetProjectDetailsList(int project_syscode, int env_syscode)
        {
            var query = from pd in context.ProjectDetails
                        join pm in context.ProjectParameterMaster on pd.parameter_syscode equals pm.parameter_syscode
                        where pd.project_syscode == project_syscode
                        && pd.env_syscode == env_syscode
                        && pd.is_active && !pd.is_deleted
                        select new
                        {
                            pd.details_syscode,
                            pd.env_syscode,
                            pm.parameter_name,
                            pd.parameter_syscode,
                            pd.parameter_value,
                            pm.editable_in_release
                        };

            List<ProjectDetails> lst = new List<ProjectDetails>();
            string lstJSON = JsonConvert.SerializeObject(query.ToList(), Newtonsoft.Json.Formatting.Indented);
            lst = JsonConvert.DeserializeObject<List<ProjectDetails>>(lstJSON);
            return lst;

        }

        public List<ProjectDetails> GetReleaseDetailsList(int project_syscode, int env_syscode)
        {

            //select pm.parameter_name, pm.parameter_syscode, pd.parameter_value,pd.project_syscode
            //from project_parameter_master pm
            //left join project_details pd on pm.parameter_syscode = pd.parameter_syscode and pd.project_syscode = 2 and pd.env_syscode = 2 and pd.is_active = 1 and pd.is_deleted = 0

            var query = from pm in context.ProjectParameterMaster
                        join pd in context.ProjectDetails on new { col1 = pm.parameter_syscode, col2 = project_syscode, col3 = env_syscode, col4 = true, col5 = false }  equals
                                                             new { col1 = pd.parameter_syscode, col2 = pd.project_syscode, col3 = pd.env_syscode, col4 = pd.is_active, col5 = pd.is_deleted }
                                                             into prmGrp
                        from pp in prmGrp.DefaultIfEmpty()                  
                        select new
                        {
                            pp.details_syscode,
                            pp.env_syscode,
                            pp.parameter_name,
                            pm.parameter_syscode,
                            pp.parameter_value,
                        };

            List<ProjectDetails> lst = new List<ProjectDetails>();
            string lstJSON = JsonConvert.SerializeObject(query.ToList(), Newtonsoft.Json.Formatting.Indented);
            lst = JsonConvert.DeserializeObject<List<ProjectDetails>>(lstJSON);
            return lst;



        }
    }
}
