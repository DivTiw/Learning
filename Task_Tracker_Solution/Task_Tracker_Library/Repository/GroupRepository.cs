using System.Collections.Generic;
using Task_Tracker_CommonLibrary.Entity;
using System.Linq;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_Library.Repository
{
    public class GroupRepository : TTBaseRepository<GroupMaster>
    {
        public GroupRepository(TTDBContext _context) : base(_context)
        {
        }

        public IList<GroupDM> GetGroupsByEmployee(int employee_syscode)
        {
            var lstGroups = (from grp in context.GroupMaster
                             join vw in context.vw_employee_master on grp.created_by equals vw.employee_syscode                        
                             where !grp.is_deleted                             
                                && context.GroupMember.Any(x => x.group_syscode == grp.group_syscode 
                                                                            && x.employee_syscode == employee_syscode 
                                                                            && x.is_active && !x.is_deleted)
                             select new GroupDM
                             {
                                 group_description = grp.group_description,
                                 group_name = grp.group_name,
                                 group_syscode = grp.group_syscode,
                                 group_email_id = grp.group_email_id,
                                 created_on = grp.created_on,
                                 is_active = grp.is_active,                                
                                 is_deleted = grp.is_deleted,
                                 created_by_Name = vw.employee_name                                
                             } 
                             ).ToList();
            return lstGroups;                            
        }
    }
}
