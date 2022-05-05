using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Library.Repository
{
    public class ReleaseRepository : TTBaseRepository<ReleaseInstructions>
    {
        public ReleaseRepository(TTDBContext _context) : base(_context)
        {
        }

        public List<ReleaseDM> getTaskReleaseReportList(int task_syscode)
        {
            List<ReleaseDM> lst = new List<ReleaseDM>();

            var query = (from rl in context.ReleaseInstructions
                   join pd in context.ProjectMaster on rl.project_syscode equals pd.project_syscode
                   join en in context.EnvironmentMaster on rl.env_syscode equals en.env_syscode
                   join ts in context.Tasks on rl.task_syscode equals ts.task_syscode
                   join vw in context.vw_employee_master on rl.created_by equals vw.employee_syscode
                   where rl.is_active && !rl.is_deleted
                   //&& rl.task_syscode == task_syscode
                   select new ReleaseDM
                   {
                       release_syscode = rl.release_syscode,
                       release_ref = rl.release_ref,
                       env_syscode = rl.env_syscode,
                       project_syscode = rl.project_syscode,
                       created_on = rl.created_on,
                       task_syscode = rl.task_syscode,
                       is_active = rl.is_active,
                       is_deleted = rl.is_deleted,
                       project = pd,
                       task = ts,
                       env = en,
                       created_by_Name = vw.employee_name,
                       is_Released = rl.is_Released,
                       Remarks = rl.Remarks,
                       modified_on = rl.modified_on,
                       lstReleaseDetailsDM = context.ReleaseDetails
                                                 .Where(r => r.release_syscode == rl.release_syscode && r.is_active && !r.is_deleted)
                                               .Join(context.ProjectParameterMaster, rd => rd.parameter_syscode, pp => pp.parameter_syscode
                                               , (rd, pp) => new ReleaseDetailsDM
                                               {
                                                   ParameterMaster = pp,
                                                   parameter_value = rd.parameter_value,
                                                   parameter_syscode = rd.parameter_syscode

                                               }).ToList()
                   });

            if (task_syscode > 0)
                query = query.Where(rel => rel.task_syscode == task_syscode);
                           
               lst = query.OrderByDescending(o=>o.created_on).ToList();


            return lst;
      
        }


        public ReleaseDM getReleaseInstructionsByID(int release_syscode)
        {
            var query = (from rl in context.ReleaseInstructions
                         join pd in context.ProjectMaster on rl.project_syscode equals pd.project_syscode
                         join en in context.EnvironmentMaster on rl.env_syscode equals en.env_syscode
                         join ts in context.Tasks on rl.task_syscode equals ts.task_syscode
                         join vw in context.vw_employee_master on rl.created_by equals vw.employee_syscode
                         where rl.is_active && !rl.is_deleted
                               && rl.release_syscode == release_syscode
                         select new ReleaseDM
                         {
                             release_syscode = rl.release_syscode,
                             task_syscode = rl.task_syscode,
                             release_ref = rl.release_ref,
                             env_syscode = rl.env_syscode,
                             project_syscode = rl.project_syscode,
                             created_on = rl.created_on,
                             project = pd,
                             task = ts,
                             env = en,
                             created_by_Name = vw.employee_name,
                             is_Released = rl.is_Released,
                             Remarks = rl.Remarks,
                             modified_on = rl.modified_on,
                             modified_by = rl.modified_by,
                             created_by = rl.created_by
                             //lstReleaseDetailsDM = context.ReleaseDetails
                             //                          .Where(r => r.release_syscode == rl.release_syscode && r.is_active && !r.is_deleted)
                             //                        .Join(context.ProjectParameterMaster, rd => rd.parameter_syscode, pp => pp.parameter_syscode
                             //                        , (rd, pp) => new ReleaseDetailsDM
                             //                        {
                             //                            ParameterMaster = pp,
                             //                            parameter_value = rd.parameter_value,
                             //                            parameter_syscode = rd.parameter_syscode

                             //                        }).ToList()
                         })
                  .FirstOrDefault();


            return query;

        }
    }
}
