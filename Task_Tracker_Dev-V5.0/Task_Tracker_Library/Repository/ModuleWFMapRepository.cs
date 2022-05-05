using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_Library.Repository
{
    //Module master as a type is just used for facilitating inheritance, none of the methods from base would be utilised that are directly casted to the ModuleMaster type.
    //Need to look for the better approach.
    public class ModuleWFMapRepository : TTBaseRepository<ModuleMaster>
    {
        public ModuleWFMapRepository(TTDBContext ModuleWFMapContext) : base(ModuleWFMapContext)
        {
        }

        public List<LevelTaskUserDTO> GetWFLevels_UsersAndWeightage(int module_Syscode)
        {
            var wfEntity = context.ModuleMaster
                            .Where(x=> x.module_syscode == module_Syscode)
                            .Join(context.WorkflowMaster,mod=> mod.workflow_syscode, wf=> wf.workflow_syscode
                                ,(mod, wf)=> new
                                            { wf.workflow_name
                                            , wf.workflow_syscode
                                            }
                            )
                            .FirstOrDefault();

            var levelTaskUserList = ( from wfld in context.WorkflowLevelDetails
                                      join tasks in context.Tasks on new { col1 = wfld.level_syscode, col2 = module_Syscode, col3 = false, col4 = true, col5 = 0 } equals new { col1 = tasks.level_syscode??0, col2 = tasks.module_syscode??0, col3 = tasks.is_deleted, col4 = tasks.is_active, col5 = tasks.parent_task_syscode ?? 0 } into leftGrp //Cols has been assigned due to type mismatch problem given by LINQ.
                                      from g in leftGrp.DefaultIfEmpty()
                                      where     wfld.workflow_syscode.Equals(wfEntity.workflow_syscode) 
                                            &&  !wfld.is_deleted && wfld.is_active 
                                      select new LevelTaskUserDTO()
                                      {
                                          level_syscode = wfld.level_syscode,
                                          task_syscode = g.task_syscode,
                                          task_ref = g.task_reference,
                                          level_name = wfld.level_name,
                                          lstUsers = context.TaskUserMapping.Where(x => x.is_active && !x.is_deleted && x.task_syscode.Equals(g.task_syscode)).Select(x => x.employee_syscode).ToList(),
                                          weightage = g.weightage
                                      }                                      
                                    ).ToList();
            if (levelTaskUserList != null)
            {
                for (int i = 0; i < levelTaskUserList.Count; i++)
                {
                    levelTaskUserList[i].workflow_name = wfEntity.workflow_name;
                    levelTaskUserList[i].arrUserSyscodes = levelTaskUserList[i].lstUsers.ToArray();
                } 
            }
            return levelTaskUserList;
        }

        public List<LevelTaskUserDTO> GetModuleLevelDetails(int module_Syscode)
        {
            var wfEntity = context.ModuleMaster
                            .Where(x => x.module_syscode == module_Syscode)
                            .Join(context.WorkflowMaster, mod => mod.workflow_syscode, wf => wf.workflow_syscode
                                , (mod, wf) => new
                                {
                                    wf.workflow_name
                                             ,
                                    wf.workflow_syscode
                                }
                            )
                            .FirstOrDefault();



            var levelTaskUserList = (from wfld in context.WorkflowLevelDetails
                                     join lvl in context.ModuleLevelDetail on new { col1 = wfld.level_syscode, col2 = module_Syscode, col3 = false, col4 = true } equals new { col1 = lvl.level_syscode, col2 = lvl.module_syscode, col3 = lvl.is_deleted, col4 = lvl.is_active } into leftGrp //Cols has been assigned due to type mismatch problem given by LINQ.
                                     from g in leftGrp.DefaultIfEmpty()
                                     where wfld.workflow_syscode.Equals(wfEntity.workflow_syscode)
                                           && !wfld.is_deleted && wfld.is_active
                                     orderby wfld.level_order ascending
                                     select new LevelTaskUserDTO()
                                     {
                                         level_syscode = wfld.level_syscode,
                                         details_syscode = g.details_syscode,
                                         level_name = wfld.level_name,
                                         lstUsers = context.ModuleLevelUserMapping.Where(x => x.is_active && !x.is_deleted && x.details_syscode.Equals(g.details_syscode)).Select(x => x.employee_syscode).ToList(),
                                         weightage = g.weightage
                                     }
                                    ).ToList();
            if (levelTaskUserList != null)
            {
                for (int i = 0; i < levelTaskUserList.Count; i++)
                {
                    levelTaskUserList[i].workflow_name = wfEntity.workflow_name;
                    levelTaskUserList[i].arrUserSyscodes = levelTaskUserList[i].lstUsers.ToArray();
                }
            }

            return levelTaskUserList;
        }
    }
}
