using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using System.Data.Objects;
using System.Data.Entity;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_Library.Repository
{
    public class DashboardRepository : TTBaseRepository<ModuleMaster>
    {
        public DashboardRepository(TTDBContext _context) : base(_context)
        {
        }

        public List<TaskDM> getTodaysLatestTasks(int userId, bool mytaskActivity = false)
        {
            DateTime dt_today = DateTime.Now;
            DateTime dt_tomorrow = DateTime.Now.AddDays(1);

            List<TaskDM> lstTasks = new List<TaskDM>();


            var query = (from userrec in context.TaskUserRecord
                         join task in context.Tasks on userrec.task_syscode equals task.task_syscode
                         join sts in context.TaskStatusMaster on task.task_status_syscode equals sts.status_syscode
                         join mod in context.ModuleMaster on new { col1 = task.module_syscode ?? 0, col2 = false, col3 = true } equals new { col1 = mod.module_syscode, col2 = mod.is_deleted, col3 = mod.is_active } into modTaskGrp
                         from modTask in modTaskGrp.DefaultIfEmpty()
                         join proj in context.ProjectMaster on new { col1 = modTask.project_syscode, col2 = false, col3 = true } equals new { col1 = proj.project_syscode, col2 = proj.is_deleted, col3 = proj.is_active } into projModGrp
                         from prjMod in projModGrp.DefaultIfEmpty()
                         join vwEmp in context.vw_employee_master on task.created_by equals vwEmp.employee_syscode into empGrp
                         from g in empGrp.DefaultIfEmpty()
                         join vwEmp1 in context.vw_employee_master on userrec.employee_syscode equals vwEmp1.employee_syscode into empGrp1
                         from g1 in empGrp1.DefaultIfEmpty()
                         where (userrec.stop_time == null && userrec.start_time >= dt_today.Date && userrec.start_time < dt_tomorrow.Date) || (userrec.stop_time >= dt_today.Date && userrec.stop_time < dt_tomorrow.Date)
                         select new TaskDM
                         {
                             task_syscode = task.task_syscode,
                             task_reference = task.task_reference,
                             status = sts,
                             project = prjMod,
                             module = modTask,
                             task_subject = task.task_subject,
                             strCreatedBy = g.employee_name,
                             target_date = task.target_date,
                             taskUserRecord = userrec,
                             strActionedBy = g1.employee_name
                         }
                        );
        
            if(mytaskActivity)
            {
                lstTasks = query.Where(x => x.taskUserRecord.employee_syscode.Equals(userId)).OrderByDescending(x => x.taskUserRecord.start_time).ToList();
            }
            else
            {
                lstTasks = query.OrderBy(x => x.strActionedBy).ToList();
            }

            for (int i = 0; i < lstTasks.Count; i++)
            {
                if (lstTasks[i].taskUserRecord.start_time != null)
                {
                    DateTime dtStart = lstTasks[i].taskUserRecord.start_time ?? DateTime.Now;
                    DateTime dtStop = lstTasks[i].taskUserRecord.stop_time ?? DateTime.Now;
                    lstTasks[i].taskUserRecord.duration = ComLibCommon.GetDurationFromHours(dtStop.Subtract(dtStart));
                }
            }

            return lstTasks;
        }

        public DashBoard getTeamActivity(DashBoard _dashInput)
        {
            DashBoard dashTeamActivity = new DashBoard();
            List<DashUsersProjects> userprojects = new List<DashUsersProjects>();
            DateTime? startdate = _dashInput.startdate;
            DateTime? enddate = _dashInput.enddate;
            int ed_syscode = 0;
            int group_syscode = 0;

            if (_dashInput.ed_syscode != null)
                ed_syscode = _dashInput.ed_syscode.Value;

            if (_dashInput.group_syscode != null)
                group_syscode = _dashInput.group_syscode.Value;


            bool isGroupHead = context.GroupMember.Any(x => x.group_syscode == group_syscode
                                                                          && x.employee_syscode == ed_syscode
                                                                          && x.is_active && !x.is_deleted
                                                                          && x.role_syscode == (int)Enum_Master.UserRoleEnum.Group_Head);
            if (!isGroupHead)
                return dashTeamActivity;

            /* --========== SQL Query for User wise Projects
 Select  emp.employee_name,p.project_name,m.module_name,
             isnull(sum((DATEDIFF(hh, tur.start_time, tur.stop_time))), 0)[hours_spent]
             from vw_employee_master emp
             --left outer Join task_user_mapping tum on emp.employee_syscode = tum.employee_syscode
             left outer join task_user_record tur on tur.task_syscode = tur.task_syscode and tur.employee_syscode = emp.employee_syscode
             left outer join task_master t on tur.task_syscode = t.task_syscode
             left outer join module_master m on t.module_syscode = m.module_syscode
             left outer join project_master p on m.project_syscode = p.project_syscode
             where emp.ed_syscode = 3986 --and emp.employee_syscode = 4918
             and (@start is null or cast(tur.start_time as date) >= @start)
             and (@end is null or cast(tur.stop_time as date) <= @end)
             group by emp.employee_name,p.project_name,m.module_name
 */

            userprojects = (from emp in context.vw_employee_master
                                //join um in context.TaskUserMapping on emp.employee_syscode equals um.employee_syscode into u1
                                //from tum in u1.DefaultIfEmpty()
                            join ur in context.TaskUserRecord on emp.employee_syscode equals ur.employee_syscode into u2
                            from tur in u2.DefaultIfEmpty()
                            join t in context.Tasks on tur.task_syscode equals t.task_syscode /*into u3
                                from t in u3.DefaultIfEmpty()*/
                            join mod in context.ModuleMaster on new { col1 = t.module_syscode ?? 0, col2 = false, col3 = true } equals new { col1 = mod.module_syscode, col2 = mod.is_deleted, col3 = mod.is_active } into modTaskGrp
                            from modTask in modTaskGrp.DefaultIfEmpty()
                            join proj in context.ProjectMaster on new { col1 = modTask.project_syscode, col2 = false, col3 = true } equals new { col1 = proj.project_syscode, col2 = proj.is_deleted, col3 = proj.is_active } into projModGrp
                            from prjMod in projModGrp.DefaultIfEmpty()
                            where /*emp.ed_syscode == ed_syscode &&*/
                            (startdate == null || DbFunctions.TruncateTime(tur.start_time.Value) >= DbFunctions.TruncateTime(startdate.Value))
                            && (enddate == null || DbFunctions.TruncateTime(tur.stop_time.Value) <= DbFunctions.TruncateTime(enddate.Value))
                            && (emp.last_working_date == null || emp.last_working_date >= DateTime.Now)
                            && emp.resigned_on == null

                            select new DashUsersProjects
                            {
                                emp_name = emp.employee_name,
                                group_syscode = prjMod.group_syscode,
                                project_name = prjMod.project_name,
                                module_name = modTask.module_name,
                                start = tur.start_time,
                                end = tur.stop_time,
                                hours_spent =
                                    (
                                        DbFunctions.DiffHours(tur.start_time, tur.stop_time).Value
                                    )
                            }).ToList();

            var userprojects1 = userprojects.Where(x => x.group_syscode != null && x.group_syscode != _dashInput.group_syscode).ToList();
            userprojects = userprojects.Except(userprojects1).ToList();

            var grouped = userprojects.GroupBy(x => new { x.emp_name, x.project_name, x.module_name })
                          .Select(g => new DashUsersProjects
                          {
                              emp_name = g.Key.emp_name,
                              project_name = g.Key.project_name,
                              module_name = g.Key.module_name,
                              hours_spent = g.Sum(a => a.hours_spent)
                          }).ToList();

            dashTeamActivity.lstUsersProjects = grouped;

            return dashTeamActivity;
        }

        public DashBoard getDashboard(DashBoard taskUser)
        {
            
            DashBoard dashboard = new DashBoard();
            List<DashWorkingUsers> workingusers = new List<DashWorkingUsers>();
            int ed_syscode = 0;
            int group_syscode = 0;

            if (taskUser.ed_syscode != null)
                ed_syscode = taskUser.ed_syscode.Value;

            if (taskUser.group_syscode != null)
                group_syscode = taskUser.group_syscode.Value;


            bool isGroupHead = context.GroupMember.Any(x => x.group_syscode == group_syscode
                                                                          && x.employee_syscode == ed_syscode
                                                                          && x.is_active && !x.is_deleted
                                                                          && x.role_syscode == (int)Enum_Master.UserRoleEnum.Group_Head);
            if (!isGroupHead)
                return dashboard;

            /* --=== SQL Query for working users on dashboard
            Select emp.employee_name,t.task_details,tur.start_time,tur.stop_time from vw_employee_master emp
            Left Outer Join task_user_record tur on emp.employee_syscode = tur.employee_syscode
            left outer join task_master t on tur.task_syscode = t.task_syscode
            where emp.ed_syscode = 3986 and
            (tur.start_time is null or  tur.start_time = (
            select
            case when((select max(start_time) from task_user_record
            Where employee_syscode = emp.employee_syscode and cast(start_time as date) = cast(getdate() as date) and stop_time is null) is null)
            then
            (select max(start_time) from task_user_record
            Where employee_syscode = emp.employee_syscode and(stop_time is null or cast(start_time as date) <> cast(getdate() as date)))
            end
            ))
            */

            workingusers = (from emp in context.vw_employee_master
                                join tu in context.TaskUserRecord on emp.employee_syscode equals tu.employee_syscode into taskuser
                                from tur in taskuser.DefaultIfEmpty()
                                join tm in context.Tasks on tur.task_syscode equals tm.task_syscode 
                                join p in context.Tasks on tm.parent_task_syscode equals p.task_syscode into parentGrp
                                from parent in parentGrp.DefaultIfEmpty()
                                join mod in context.ModuleMaster on new { col1 = tm.module_syscode ?? 0, col2 = false, col3 = true } equals new { col1 = mod.module_syscode, col2 = mod.is_deleted, col3 = mod.is_active } into modTaskGrp
                                from modTask in modTaskGrp.DefaultIfEmpty()
                                join proj in context.ProjectMaster on new { col1 = modTask.project_syscode, col2 = false, col3 = true } equals new { col1 = proj.project_syscode, col2 = proj.is_deleted, col3 = proj.is_active } into projModGrp
                                from prjMod in projModGrp.DefaultIfEmpty()
                                where (tur.start_time.Value.Equals(
                                    ((from a in context.TaskUserRecord
                                      where a.employee_syscode == emp.employee_syscode
                                      && (a.start_time.Value == DateTime.Now)
                                      && a.stop_time.Value == null
                                      select a.start_time.Value).Max()) == null ?
                                     ((from a in context.TaskUserRecord
                                       where a.employee_syscode == emp.employee_syscode
                                       && (a.stop_time.Value == null || (DateTime?)a.start_time.Value != DateTime.Now)
                                       select a.start_time.Value).Max()) :
                                                               ((from a in context.TaskUserRecord
                                                                 where a.employee_syscode == emp.employee_syscode
                                                                && (DateTime?)a.start_time.Value == DateTime.Now
                                                                 && (DateTime?)a.stop_time.Value == null
                                                                 select a.start_time.Value).Max())
                                    ))
                                //&& emp.ed_syscode == ed_syscode
                                && (emp.last_working_date == null || emp.last_working_date >= DateTime.Now)
                                && emp.resigned_on == null

                            select new DashWorkingUsers
                                {
                                    task_syscode = tm.task_syscode != null ? tm.task_syscode : 0,
                                    group_syscode = prjMod.group_syscode,
                                    project_name = prjMod.project_name,
                                    module_name = modTask.module_name,
                                    task_subject = tm.task_subject,
                                    parent_task_subject = parent.task_subject,
                                    emp_name = emp.employee_name,
                                    start_time = tur.start_time,
                                    stop_time = tur.stop_time,
                                    //duration = ComLibCommon.GetDurationFromHours((tur.stop_time - tur.stop_time)??TimeSpan.Zero),
                                    isWorking =
                                       (
                                           (tur.start_time.Value == null && tur.stop_time.Value == null) ? false :
                                           (tur.start_time.Value != null && tur.stop_time.Value == null) ? true :
                                           (tur.start_time.Value != null && tur.stop_time.Value != null) ? false : false
                                       )
                                })
                                .OrderByDescending(x => x.isWorking).ThenByDescending(x => x.start_time)
                                .ToList();

            var workingusers1 = workingusers.Where(x => x.group_syscode != null && x.group_syscode != taskUser.group_syscode).ToList();
            workingusers = workingusers.Except(workingusers1).ToList();

            for (int i = 0; i < workingusers.Count; i++)
            {
                if (workingusers[i].start_time != null)
                {
                    DateTime dtStart = workingusers[i].start_time ?? DateTime.Now;
                    DateTime dtStop = workingusers[i].stop_time ?? DateTime.Now;
                    workingusers[i].duration = ComLibCommon.GetDurationFromHours(dtStop.Subtract(dtStart));
                }
            }

            dashboard.lstWorkingUsers = workingusers;

            return dashboard;
        }

    }
}
