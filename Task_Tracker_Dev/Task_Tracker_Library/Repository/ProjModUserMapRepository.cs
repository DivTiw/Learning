using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_Library.Repository
{
    public class ProjModUserMapRepository : TTBaseRepository<ProjModUserMapping>
    {
        public ProjModUserMapRepository(TTDBContext _context) : base(_context)
        {
        }

        public int[] getModuleWriteUsers(int module_syscode)
        {
            var query = (from usr in context.ProjModUserMapping
                         where usr.module_syscode == module_syscode
                            && usr.role_syscode == (int)Enum_Master.UserRoleEnum.Module_User
                            && usr.access_write == true
                            && usr.is_active == true
                            && usr.is_deleted == false
                         select new ProjModUserMapping
                         {
                             employee_syscode = usr.employee_syscode

                         }).ToList();

            return query?.Select(x => x.employee_syscode).ToArray();
        }
        public int[] getModuleReadUsers(int module_syscode)
        {
            var query = (from usr in context.ProjModUserMapping
                         where usr.module_syscode == module_syscode
                            && usr.role_syscode == (int)Enum_Master.UserRoleEnum.Module_User
                            && usr.access_read == true
                            && usr.access_write == false
                            && usr.is_active == true
                            && usr.is_deleted == false
                         select new ProjModUserMapping
                         {
                             employee_syscode = usr.employee_syscode

                         }).ToList();

            return query?.Select(x => x.employee_syscode).ToArray();
        }
        public int[] getAllModuleUsers(int module_syscode)
        {
            var query = (from usr in context.ProjModUserMapping
                         where usr.module_syscode == module_syscode
                            && usr.role_syscode == (int)Enum_Master.UserRoleEnum.Module_User
                            && usr.is_active == true
                            && usr.is_deleted == false
                         select new ProjModUserMapping
                         {
                             employee_syscode = usr.employee_syscode

                         }).ToList();

            return query?.Select(x => x.employee_syscode).ToArray();
        }
        public int[] getProjectWriteUsers(int project_syscode)
        {
            var query = (from usr in context.ProjModUserMapping
                         where usr.project_syscode == project_syscode
                            && usr.role_syscode == (int)Enum_Master.UserRoleEnum.Project_User
                            && usr.access_write == true
                            && usr.is_active == true
                            && usr.is_deleted == false
                         select new ProjModUserMapping
                         {
                             employee_syscode = usr.employee_syscode

                         }).ToList();

            return query?.Select(x => x.employee_syscode).ToArray();
        }
        public int[] getProjectReadUsers(int project_syscode)
        {
            var query = (from usr in context.ProjModUserMapping
                         where usr.project_syscode == project_syscode
                            && usr.role_syscode == (int)Enum_Master.UserRoleEnum.Project_User
                            && usr.access_read == true
                            && usr.access_write == false
                            && usr.is_active == true
                            && usr.is_deleted == false
                         select new ProjModUserMapping
                         {
                             employee_syscode = usr.employee_syscode

                         }).ToList();

            return query?.Select(x => x.employee_syscode).ToArray();
        }
        public int[] getAllProjectUsers(int project_syscode)
        {
            var query = (from usr in context.ProjModUserMapping
                         where usr.project_syscode == project_syscode
                            && usr.role_syscode == (int)Enum_Master.UserRoleEnum.Project_User
                            && usr.is_active == true
                            && usr.is_deleted == false
                         select new ProjModUserMapping
                         {
                             employee_syscode = usr.employee_syscode

                         }).ToList();

            return query?.Select(x => x.employee_syscode).ToArray();
        }
        public int[] getProjModWriteUsers(int _modSyscode, int _projSyscode)
        {
            var _arrWriteUsers = (from prjMod in context.ProjModUserMapping
                                  where ((prjMod.project_syscode == _projSyscode
                                                            && prjMod.module_syscode == null
                                                            && prjMod.role_syscode == (int)Enum_Master.UserRoleEnum.Project_User
                                                           ) ||
                                                           (prjMod.project_syscode == _projSyscode
                                                            && prjMod.module_syscode == _modSyscode
                                                            && prjMod.role_syscode == (int)Enum_Master.UserRoleEnum.Module_User
                                                           ))
                                                           && prjMod.access_write && prjMod.is_active && !prjMod.is_deleted
                                  select prjMod.employee_syscode)?.ToArray();
            return _arrWriteUsers;
        }
        public int[] getProjModReadUsers(int _modSyscode, int _projSyscode)
        {
            var _arrReadUsers = (from prjMod in context.ProjModUserMapping
                                 where ((prjMod.project_syscode == _projSyscode
                                                           && prjMod.module_syscode == null
                                                           && prjMod.role_syscode == (int)Enum_Master.UserRoleEnum.Project_User
                                                          ) ||
                                                          (prjMod.project_syscode == _projSyscode
                                                           && prjMod.module_syscode == _modSyscode
                                                           && prjMod.role_syscode == (int)Enum_Master.UserRoleEnum.Module_User
                                                          ))
                                                          && !prjMod.access_write && prjMod.access_read && prjMod.is_active && !prjMod.is_deleted
                                 select prjMod.employee_syscode)?.ToArray();
            return _arrReadUsers;
        }
        public int[] getAllProjModUsers(int _modSyscode, int _projSyscode)
        {
            var _arrAllUsers = (from prjMod in context.ProjModUserMapping
                                where ((prjMod.project_syscode == _projSyscode
                                                          && prjMod.module_syscode == null
                                                          && prjMod.role_syscode == (int)Enum_Master.UserRoleEnum.Project_User
                                                         ) ||
                                                         (prjMod.project_syscode == _projSyscode
                                                          && prjMod.module_syscode == _modSyscode
                                                          && prjMod.role_syscode == (int)Enum_Master.UserRoleEnum.Module_User
                                                         ))
                                                         && prjMod.is_active && !prjMod.is_deleted
                                select prjMod.employee_syscode)?.ToArray();
            return _arrAllUsers;
        }

    }
}
