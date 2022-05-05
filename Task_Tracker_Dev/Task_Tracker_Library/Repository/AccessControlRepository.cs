using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_Library.Repository
{
    public class AccessControlRepository : TTBaseRepository<ProjModUserMapping>
    {
        public AccessControlRepository(TTDBContext _context) : base(_context)
        {
        }

        /// <summary>
        /// It returns true for write access if the logged in user having write access in records found or else he/she is creator of the record.
        /// If no records found then it by default returns false for the write access.
        /// </summary>
        /// <param name="empSyscode"></param>
        /// <param name="projectSyscode"></param>
        /// <returns></returns>
        public bool returnProjectAccess(int empSyscode, int projectSyscode)
        {
            bool isWriteAccess = false;

            if (empSyscode == 0)
                return false;
            if (projectSyscode == 0) //If project syscode is zero that means this is standalone task.
                return true;

            bool isCreator = context.ProjectMaster.Any(x => x.created_by == empSyscode && x.project_syscode == projectSyscode);
            if (isCreator)
                return true;

            var query = (from access in context.ProjModUserMapping
                         where access.project_syscode == projectSyscode && access.employee_syscode == empSyscode
                             && access.role_syscode == (int)Enum_Master.UserRoleEnum.Project_User
                             && access.is_active && !access.is_deleted
                         select access).FirstOrDefault();
            isWriteAccess = query?.access_write ?? false;
            return isWriteAccess;
        }

        public bool returnGroupAccess(int employee_syscode, int group_syscode)
        {
            bool isWriteAccess = false;

            bool isCreator = context.GroupMaster.Any(x => x.created_by == employee_syscode && x.group_syscode == group_syscode);
            if (isCreator)
                return true;

            bool isGroupHead = context.GroupMember.Any(x => x.group_syscode == group_syscode
                                                                            && x.employee_syscode == employee_syscode
                                                                            && x.is_active && !x.is_deleted
                                                                            && x.role_syscode == (int)Enum_Master.UserRoleEnum.Group_Head);
            if (isGroupHead)
                return true;

            return isWriteAccess;

        }

        /// <summary>
        /// It returns true for write access if the logged in user having write access in records found or else he/she is creator of the record.
        /// If no records found then it by default returns false for the write access.
        /// </summary>
        /// <param name="empSyscode"></param>
        /// <param name="moduleSyscode"></param>
        /// <returns></returns>
        public bool returnModuleAccess(int empSyscode, int moduleSyscode)
        {
            bool isWriteAccess = false;

            if (empSyscode == 0)
                return false;
            if (moduleSyscode == 0) //If module syscode is zero that means this is standalone task.
                return true;

            bool isCreator = context.ModuleMaster.Any(x => x.created_by == empSyscode && x.module_syscode == moduleSyscode);
            if (isCreator)
                return true;

            var query = (from access in context.ProjModUserMapping
                         where access.module_syscode == moduleSyscode && access.employee_syscode == empSyscode
                             && access.role_syscode == (int)Enum_Master.UserRoleEnum.Module_User
                             && access.is_active && !access.is_deleted
                         select access).FirstOrDefault();
            isWriteAccess = query?.access_write ?? false;
            return isWriteAccess;
        }

        public bool returnTaskAccess(int empSyscode, int taskSyscode, int groupSyscode)
        {
            JMTask curTask = context.Tasks.SingleOrDefault(x => x.task_syscode == taskSyscode);
            if (curTask == null)
                return false;

            bool isGroupHead = context.GroupMember.Any(x => x.group_syscode == groupSyscode
                                                                          && x.employee_syscode == empSyscode
                                                                          && x.is_active && !x.is_deleted
                                                                          && x.role_syscode == (int)Enum_Master.UserRoleEnum.Group_Head);

            if (curTask.task_status_syscode == (int)Enum_Master.StatusEnum.Complete || curTask.task_status_syscode == (int)Enum_Master.StatusEnum.Discard)
                return false;
            else if (isGroupHead)
                return true;
            else if (empSyscode == curTask.created_by || empSyscode == curTask.task_owner || empSyscode == curTask.task_on_behalf || context.TaskUserMapping.Any(x => x.employee_syscode == empSyscode && x.task_syscode == taskSyscode && x.is_active && !x.is_deleted))
                return true;
            else if (returnModuleAccess(empSyscode, curTask.module_syscode ?? 0))
                return true;
            else
            {
                int projectSyscode = context.ModuleMaster.SingleOrDefault(x => x.module_syscode == curTask.module_syscode)?.project_syscode ?? 0;
                if (returnProjectAccess(empSyscode, projectSyscode))
                    return true;
                else
                    return false;
            }
        }
    }
}
