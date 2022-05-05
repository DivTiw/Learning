using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Library.Repository
{
    public class ModuleRepository : TTBaseRepository<ModuleMaster>
    {
        public ModuleRepository(TTDBContext _context) : base(_context)
        {
        }

        public List<ModuleDM> getModuleList(int project_syscode)
        {
            var query = (from mod in context.ModuleMaster
                         join cat in context.CategoryMaster on mod.category_syscode equals cat.category_syscode
                         join proj in context.ProjectMaster on mod.project_syscode equals proj.project_syscode
                         join vwEmp in context.vw_employee_master on mod.created_by equals vwEmp.employee_syscode into empGrp
                         from g in empGrp.DefaultIfEmpty()
                         where mod.is_deleted == false && mod.is_active == true 
                               && proj.is_deleted == false && proj.is_active == true
                               && mod.project_syscode == project_syscode                               
                         select new ModuleDM
                         {
                             project_syscode = mod.project_syscode,
                             module_syscode = mod.module_syscode,
                             module_name = mod.module_name,
                             module_description =mod.module_description,
                             is_deleted = mod.is_deleted,
                             is_active = mod.is_active,
                             created_by = mod.created_by,
                             created_on = mod.created_on,
                             category_name = cat.category_name,                            
                             created_by_Name = g.employee_name,
                             project_name = proj.project_name                         
                         });       
       

            return query.OrderByDescending(x => x.created_on).ToList();
        }

        public ModuleDM getModuleById(int module_syscode)
        {
            ModuleDM modDM = new ModuleDM();

            modDM = (from mod in context.ModuleMaster
                         join cat in context.CategoryMaster on mod.category_syscode equals cat.category_syscode
                         join proj in context.ProjectMaster on mod.project_syscode equals proj.project_syscode
                         where mod.is_deleted == false && mod.module_syscode == module_syscode
                         select new ModuleDM
                         {
                             module_syscode = mod.module_syscode,
                             module_name = mod.module_name,
                             module_description = mod.module_description,
                             category_syscode = mod.category_syscode,
                             workflow_syscode = mod.workflow_syscode,
                             is_deleted = mod.is_deleted,
                             is_active = mod.is_active,
                             created_by = mod.created_by,
                             created_on = mod.created_on,
                             category_name = cat.category_name,
                             project_name = proj.project_name,
                             project_syscode = mod.project_syscode
                         }).FirstOrDefault();

            return modDM;

        }

        public List<ModuleDM> getProjectModuleByName(string mod_name, int proj_syscode)
        {
            //ModuleDM modDM = new ModuleDM();

            var modDM = (from mod in context.ModuleMaster
                         join proj in context.ProjectMaster on mod.project_syscode equals proj.project_syscode
                         where !mod.is_deleted && mod.is_active && mod.module_name == mod_name && proj.project_syscode == proj_syscode
                         select new ModuleDM
                         {
                             module_syscode = mod.module_syscode,
                             module_name = mod.module_name,
                             module_description = mod.module_description,
                             category_syscode = mod.category_syscode,
                             workflow_syscode = mod.workflow_syscode,
                             is_deleted = mod.is_deleted,
                             is_active = mod.is_active,
                             created_by = mod.created_by,
                             created_on = mod.created_on,
                             //category_name = cat.category_name,
                             project_name = proj.project_name,
                             project_syscode = mod.project_syscode
                         }).ToList();

            return modDM;
        }
    }
}
