using Common_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Library.Interface;
using Task_Tracker_Library.Repository;
using Task_tracker_WebAPI.Controllers;

namespace Task_tracker_WebAPI.Areas.Master.Controllers
{
    public class ModuleController : BaseAPIController
    {
        [HttpPost] //Not used
        public ProjectMaster AddUpdateProjectModules([FromBody] IList<ModuleMaster> modules)
        {
            ProjectMaster pm = new ProjectMaster();

            if (modules.Count == 0)
            {
                pm.opStatus = false;
                pm.opMsg = "Invalid Project";
            }
            try
            {
                using (var uow = new UnitOfWork())
                {
                    foreach (var module in modules)
                    {
                        OperationDetailsDTO od = new OperationDetailsDTO();

                        //System.Data.Entity.EntityState operation;
                        if (module.module_syscode > 0)
                            uow.ModuleRepo.Update(module);
                        //operation = System.Data.Entity.EntityState.Modified;
                        else
                            uow.ModuleRepo.Add(module);
                        //operation = System.Data.Entity.EntityState.Added;

                        //od = uow.ModuleRepo.saveOperation(module, operation);                                             
                    }
                    uow.commitTT();
                    pm.lstModules = modules;
                    pm.opStatus = true;
                    pm.opMsg = "Modules added successfully. ";
                    //if (od.opStatus)
                    //{
                    //    module.opStatus = true;
                    //    if (pm.lstModules == null)
                    //        pm.lstModules = new List<ModuleMaster>();

                    //pm.lstModules.Add(module);
                    //pm.opStatus = true;
                    //    pm.opMsg = od.opMsg;
                    //}
                    //else throw new Exception(od.opMsg);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", "", "ProjectModules", "ProjectController");
                pm.opStatus = false;
                pm.opMsg = ex.Message;
            }
            return pm;
        }


        [HttpPost]
        public IList<ModuleDM> GetModuleList([FromBody] ModuleMaster module)
        {
            IList<ModuleDM> mList = null;
            try
            {
                mList = new List<ModuleDM>();
                using (var uow = new UnitOfWork())
                {
                    bool blnProjectAccess = uow.AccessControlRepo.returnProjectAccess(module.logged_in_user, module.project_syscode);
                    mList = uow.ModuleRepo.getModuleList(module.project_syscode);

                    if (mList != null && mList.Count > 0)
                    {
                        for (int i = 0; i < mList.Count; i++)
                        {
                            mList[i].RecordHasWriteAccess = uow.AccessControlRepo.returnModuleAccess(module.logged_in_user, mList[i].module_syscode)
                                                         || uow.AccessControlRepo.returnProjectAccess(module.logged_in_user, mList[i].project_syscode);
                            mList[i].blnProjectWriteAccess = blnProjectAccess;
                        }
                    }
                    else
                    {
                        mList.Add(new ModuleDM() { project_syscode = module.project_syscode, logged_in_user = module.logged_in_user, blnProjectWriteAccess = blnProjectAccess });
                    }
                }
            }
            catch (Exception ex)
            {
                Exception e = ex.ReturnActualException();
                Log.LogError(e.Message, "", null, "GetModuleList", "ModuleController");
            }


            return mList;
        }

        [HttpPost]
        public OperationDetailsDTO GetModuleByID([FromBody] ModuleDM module)
        {
            OperationDetailsDTO od = new OperationDetailsDTO();
            ModuleDM modDM = null;

            if (module.module_syscode <= 0) throw new Exception("Invalid Module.");

            try
            {
                using (var uow = new UnitOfWork())
                {
                    modDM = uow.ModuleRepo.getModuleById(module.module_syscode);
                    modDM.PageHasWriteAccess = uow.AccessControlRepo.returnModuleAccess(module.logged_in_user, module.module_syscode)
                                            || uow.AccessControlRepo.returnProjectAccess(module.logged_in_user, modDM.project_syscode);
                    if (modDM == null)
                        throw new Exception("Some Error occured while fetching module data.");


                    modDM.ddlData = module.ddlData; //Again replacing with original ddlData object as it was replaced by the auto mapper while mapping from the entity to domain model.

                    modDM.arrReadUserSyscodes = uow.ProjModUserMappingRepo.GetList(x => x.is_deleted == false
                                                                                  && x.project_syscode.Equals(modDM.project_syscode)
                                                                                  && x.module_syscode.Equals(modDM.module_syscode)
                                                                                  && x.access_read
                                                                                  && x.access_write == false
                                                                                  && x.is_active
                                                                                 )
                                                                        .Select(x => x.employee_syscode).ToArray();

                    modDM.arrWriteUserSyscodes = uow.ProjModUserMappingRepo.GetList(x => x.is_deleted == false
                                                                                && x.project_syscode.Equals(modDM.project_syscode)
                                                                                && x.module_syscode.Equals(modDM.module_syscode)
                                                                                && x.access_write
                                                                                && x.is_active
                                                                               )
                                                                      .Select(x => x.employee_syscode).ToArray();
                    uow.CommonRepo.fillDDLdata(modDM.ddlData);  //wf.ddlData.fillDDLdata(ComRepo.fillDDLdata);

                    modDM.opStatus = true;
                }

            }
            catch (Exception ex)
            {
                Exception e = ex.ReturnActualException();

                Log.LogError(e.Message, "", null, "GetProjectByID", "ProjectController");
                od.opStatus = false;
                od.opMsg = e.Message;
                od.opInnerException = e;
            }

            return modDM;
        }

        [HttpPost]
        public ModuleMaster PostModule([FromBody] ModuleDM modDM)
        {
            string returnvalue = string.Empty;
            OperationDetailsDTO od = new OperationDetailsDTO();
            ModuleMaster module = null;

            if (string.IsNullOrEmpty(modDM.module_name))
            {
                od.opStatus = false;
                od.opMsg = "Invalid Module";
            }
            try
            {
                if (ComLibCommon.CheckDuplicateUser(modDM.arrReadUserSyscodes, modDM.arrWriteUserSyscodes))
                {
                    throw new Exception("Read Access User cannot be added as Write Access User.");
                }
                using (var uow = new UnitOfWork())
                {
                    var mod = uow.ModuleRepo.getProjectModuleByName(modDM.module_name, modDM.project_syscode);
                    if (mod != null && mod.Count > 0)
                    {
                        throw new Exception("Module with name " + modDM.module_name + " already exists in this Group.");
                    }
                    module = new ModuleMaster();
                    module.module_name = modDM.module_name;
                    module.module_description = modDM.module_description;
                    module.project_syscode = modDM.project_syscode;
                    module.workflow_syscode = modDM.workflow_syscode;
                    module.category_syscode = modDM.category_syscode;
                    module.created_by = modDM.created_by;
                    module.is_active = modDM.is_active;
                    module.is_deleted = modDM.is_deleted;

                    uow.ModuleRepo.Add(module);
                    uow.commitTT();

                    modDM.module_syscode = module.module_syscode;

                    od = SaveProjectModuleMembers(modDM);
                    if (od.opStatus)
                    {
                        modDM.opStatus = true;
                        modDM.opMsg = "Module created successfully!";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", modDM.created_by.ToString(), "PostModule", "ModuleController");
                modDM.opStatus = false;
                modDM.opMsg = ex.Message;
                modDM.opInnerException = ex;
            }
            return modDM;
        }

        [HttpPut]
        public OperationDetailsDTO PutModule([FromBody] ModuleDM modDM)
        {
            ModuleMaster mod = null;

            OperationDetailsDTO od = new OperationDetailsDTO();
            if (modDM == null || modDM.module_syscode == 0)
            {
                od.opStatus = false;
                od.opMsg = "Invalid Module";
            }
            try
            {
                if (ComLibCommon.CheckDuplicateUser(modDM.arrReadUserSyscodes, modDM.arrWriteUserSyscodes))
                {
                    throw new Exception("Read Access User cannot be added as Write Access User.");
                }
                using (var uow = new UnitOfWork())
                {
                    if (uow.TasksRepo.Any(x => x.module_syscode == modDM.module_syscode))
                    {
                        od.opStatus = false;
                        od.opMsg = "This module is already used for task creation and can not be modified.";
                        od.opUserFrndlyMsg = "This Module is already in use so can not be modified.";
                        return od;
                    }


                    var module = uow.ModuleRepo.getProjectModuleByName(modDM.module_name, modDM.project_syscode);
                    if (module != null) /**/
                    {
                        if (module.Count > 1 || (module.Count == 1 && (module[0].module_syscode != modDM.module_syscode)))
                            throw new Exception("Module with name " + modDM.module_name + " already exists in this Group");
                    }

                    mod = uow.ModuleRepo.Get(modDM.module_syscode);

                    //This is for deleting the existing level and user mapping details when the workflow is changed in the module.
                    #region DeleteDetailsWhenWorkflowChanges
                    if (mod.workflow_syscode != modDM.workflow_syscode)
                    {
                        var modLvlMap = uow.ModuleLevelDetailRepo.GetList(x => x.module_syscode == modDM.module_syscode && x.is_active && !x.is_deleted).ToList();
                        if (modLvlMap != null)
                        {
                            foreach (var level in modLvlMap)
                            {
                                level.is_active = false;
                                level.is_deleted = true;
                                level.modified_by = modDM.created_by;
                                level.modified_on = DateTime.Now;
                                uow.ModuleLevelDetailRepo.Update(level);

                                List<ModuleLevelUserMapping> UserMappingsinDB = uow.ModuleLevelUserMappingRepo
                                                                            .GetList(x => x.details_syscode == level.details_syscode
                                                                                        && !x.is_deleted
                                                                                        && x.is_active)
                                                                            ?.ToList();
                                UserMappingsinDB.ForEach(x => { x.is_deleted = true; x.is_active = false; x.modified_by = modDM.created_by; x.modified_on = DateTime.Now; });

                                uow.ModuleLevelUserMappingRepo.UpdateRange(UserMappingsinDB);
                            }
                        }
                    }
                    #endregion

                    mod.module_name = modDM.module_name;
                    mod.module_description = modDM.module_description;
                    mod.category_syscode = modDM.category_syscode;
                    mod.workflow_syscode = modDM.workflow_syscode;
                    mod.is_active = modDM.is_active;
                    mod.modified_by = modDM.modified_by;

                    uow.ModuleRepo.Update(mod);
                    uow.commitTT();

                    od = SaveProjectModuleMembers(modDM);

                    if (od.opStatus)
                    {
                        modDM.opStatus = true;
                        modDM.opMsg = "Module updated successfully!";
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", modDM.created_by.ToString(), "PutModule", "ModuleController");
                od.opStatus = false;
                od.opMsg = ex.Message;
                od.opInnerException = ex;
                modDM.opStatus = false;
                modDM.opMsg = ex.Message;
                modDM.opInnerException = ex;
            }
            return modDM;
        }

        [HttpPost]
        public WorkflowMaster GetModuleWorkflow(ModuleMaster mm)
        {
            WorkflowMaster wlf = null;
            //ICommonRepository<WorkflowMaster> ComRepo = new MasterRepository<WorkflowMaster>(new TTDBContext());
            try
            {
                using (var uow = new UnitOfWork())
                {
                    wlf = uow.WorkflowRepo.GetList(x => x.workflow_syscode.Equals(mm.workflow_syscode) && x.is_active && x.is_deleted == false, x => x.lstWFLevels)?.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", null, "GetProjectModulesList", "ModuleController");
            }
            return wlf;
        }

        private OperationDetailsDTO SaveProjectModuleMembers(ModuleDM moduleDM)
        {
            OperationDetailsDTO od = new OperationDetailsDTO();
            using (var uow = new UnitOfWork())
            {
                if (moduleDM.arrReadUserSyscodes != null)
                {
                    List<ProjModUserMapping> lstReadUsers = new List<ProjModUserMapping>();

                    foreach (var arrItem in moduleDM.arrReadUserSyscodes)
                    {
                        ProjModUserMapping user = new ProjModUserMapping();
                        user.employee_syscode = arrItem;
                        user.role_syscode = (int)Enum_Master.UserRoleEnum.Module_User;   //Enum.GetName(typeof(TEnum), value)
                        user.project_syscode = moduleDM.project_syscode;
                        user.module_syscode = moduleDM.module_syscode;
                        user.access_read = true;
                        user.created_by = moduleDM.created_by;
                        lstReadUsers.Add(user);
                    }

                    List<ProjModUserMapping> ProjModReadUsersinDB = uow.ProjModUserMappingRepo
                                                                    .GetList(x => x.project_syscode.Equals(moduleDM.project_syscode)
                                                                                && x.module_syscode.Equals(moduleDM.module_syscode)
                                                                                && x.role_syscode == (int)Enum_Master.UserRoleEnum.Module_User
                                                                                && x.access_read
                                                                                && !x.access_write
                                                                                && !x.is_deleted
                                                                                && x.is_active)
                                                                    ?.ToList();
                    List<ProjModUserMapping> lstNewReadUsr = null;
                    if (ProjModReadUsersinDB != null && ProjModReadUsersinDB.Count > 0)
                    {
                        //List of users removed from mapping from the UI and needs to be deleted and made is active false.
                        List<ProjModUserMapping> lstDelTskUsr = ProjModReadUsersinDB.Where(x => !lstReadUsers.Any(y => y.employee_syscode.Equals(x.employee_syscode))).ToList();

                        //List of users new and not present in mapping table.
                        lstNewReadUsr = lstReadUsers.Where(x => !ProjModReadUsersinDB.Any(y => y.employee_syscode.Equals(x.employee_syscode))).ToList();

                        //Process for deleting the mappings which are not present in the UI. Changing the property value of list to make it deleted.
                        lstDelTskUsr.ForEach(x => { x.is_deleted = true; x.is_active = false; x.modified_by = moduleDM.created_by; x.modified_on = DateTime.Now; });

                        uow.ProjModUserMappingRepo.UpdateRange(lstDelTskUsr);
                    }
                    else
                    {
                        lstNewReadUsr = lstReadUsers;
                    }
                    uow.ProjModUserMappingRepo.AddRange(lstNewReadUsr);
                    uow.commitTT();
                }
                if (moduleDM.arrWriteUserSyscodes != null)
                {
                    List<ProjModUserMapping> lstWriteUsers = new List<ProjModUserMapping>();

                    foreach (var arrItem in moduleDM.arrWriteUserSyscodes)
                    {
                        ProjModUserMapping user = new ProjModUserMapping();
                        user.employee_syscode = arrItem;
                        user.role_syscode = (int)Enum_Master.UserRoleEnum.Module_User;   //Enum.GetName(typeof(TEnum), value)
                        user.project_syscode = moduleDM.project_syscode;
                        user.module_syscode = moduleDM.module_syscode;
                        user.access_read = true;
                        user.access_write = true;
                        user.created_by = moduleDM.created_by;
                        lstWriteUsers.Add(user);
                    }

                    List<ProjModUserMapping> ProjModWriteUsersinDB = uow.ProjModUserMappingRepo
                                                                    .GetList(x => x.project_syscode.Equals(moduleDM.project_syscode)
                                                                                && x.module_syscode.Equals(moduleDM.module_syscode)
                                                                                && x.role_syscode == (int)Enum_Master.UserRoleEnum.Module_User
                                                                                && x.access_write
                                                                                && !x.is_deleted
                                                                                && x.is_active)
                                                                    ?.ToList();
                    List<ProjModUserMapping> lstNewWriteUsr = null;
                    if (ProjModWriteUsersinDB != null && ProjModWriteUsersinDB.Count > 0)
                    {
                        //List of users removed from mapping from the UI and needs to be deleted and made is active false.
                        List<ProjModUserMapping> lstDelUsr = ProjModWriteUsersinDB.Where(x => !lstWriteUsers.Any(y => y.employee_syscode.Equals(x.employee_syscode))).ToList();

                        //List of users new and not present in mapping table.
                        lstNewWriteUsr = lstWriteUsers.Where(x => !ProjModWriteUsersinDB.Any(y => y.employee_syscode.Equals(x.employee_syscode))).ToList();

                        //Process for deleting the mappings which are not present in the UI. Changing the property value of list to make it deleted.
                        lstDelUsr.ForEach(x => { x.is_deleted = true; x.is_active = false; x.modified_by = moduleDM.created_by; x.modified_on = DateTime.Now; });

                        uow.ProjModUserMappingRepo.UpdateRange(lstDelUsr);
                    }
                    else
                    {
                        lstNewWriteUsr = lstWriteUsers;
                    }
                    uow.ProjModUserMappingRepo.AddRange(lstNewWriteUsr);
                    uow.commitTT();
                }

                od.opStatus = true;
                od.opMsg = "Members successfully updated.";
                return od;
            }
        }
    }
}
