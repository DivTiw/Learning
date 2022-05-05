//using Common_Components;
using Common_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_Library.Interface;
using Task_Tracker_Library.Repository;
using Task_Tracker_CommonLibrary.Utility;
using Task_tracker_WebAPI.Controllers;
using Task_Tracker_CommonLibrary.Others;

namespace Task_tracker_WebAPI.Areas.Master.Controllers
{
    public class ProjectController : BaseAPIController
    {

        [HttpPost]

        public IList<ProjectMaster> GetAllProjectList([FromBody] ProjectMaster project)
        {
            IList<ProjectMaster> mList = null;
            try
            {
                mList = new List<ProjectMaster>();
                using (var uow = new UnitOfWork())
                {
                    if (project.project_syscode > 0)
                        mList = uow.ProjectsRepo.GetList(x => x.is_deleted == false 
                                                            && x.project_syscode == project.project_syscode
                                                            && x.group_syscode.Equals(project.group_syscode)
                                                        )
                                                        //.Join(uow.EmployeeRepo.GetList(x=> uow.GroupMemberRepo.GetList(y=> y.group_syscode == project.group_syscode).Select(z=> z.employee_syscode).Contains(x.employee_syscode)),proj => proj.created_by, emp => emp.employee_syscode, (proj, emp) => new
                                                        //{
                                                        //    taskRef = usrtsk.task_reference
                                                        //})
                                                        .ToList();                  
                    else                    
                        mList = uow.ProjectsRepo.GetList(x => x.is_deleted == false && x.group_syscode.Equals(project.group_syscode)).ToList();

                    var empNameDic = uow.EmployeeRepo.GetEmpNamesBySyscode(mList.Select(x=> x.created_by).Distinct().ToList());

                    if (empNameDic == null || empNameDic.Count <= 0) empNameDic = new Dictionary<int, string>();

                    for (int i = 0; i < mList.Count; i++)
                    {
                        int creator = mList[i].created_by;
                        mList[i].created_by_Name = empNameDic.ContainsKey(creator) ? empNameDic[creator] : string.Empty;

                        mList[i].RecordHasWriteAccess = uow.AccessControlRepo.returnProjectAccess(project.logged_in_user, mList[i].project_syscode);
                    }
                }
            }
            catch (Exception ex)
            {
                Exception e = ex.ReturnActualException();                
                Log.LogError(e.Message, "", null, "GetAllProjectList", "ProjectController");
            }


            return mList;
        }

        [HttpPost]
        public OperationDetailsDTO PostProject([FromBody] ProjectDM projDM)
        {
            OperationDetailsDTO od = new OperationDetailsDTO();
            if (string.IsNullOrEmpty(projDM.project_name))
            {
                od.opStatus = false;
                od.opMsg = "Invalid Project";
            }
            try
            {
                if(ComLibCommon.CheckDuplicateUser(projDM.arrReadUserSyscodes, projDM.arrWriteUserSyscodes) )
                {
                    throw new Exception("Read Access User cannot be added as Write Access User.");
                }
                using (var uow = new UnitOfWork())
                {
                    ProjectMaster proj = uow.ProjectsRepo.GetSingle(x => x.is_deleted == false 
                                                                      && x.project_name.Equals(projDM.project_name)
                                                                      && x.group_syscode == projDM.group_syscode);
                    if(proj != null)
                    {
                        throw new Exception("Project with name " + projDM.project_name + " already exists in this Group.");
                    }
                    proj = new ProjectMaster();
                    proj.group_syscode = projDM.group_syscode;
                    proj.project_name = projDM.project_name;
                    proj.project_description = projDM.project_description;
                    proj.created_by = projDM.created_by;
                    proj.is_active = projDM.is_active;
                    proj.is_deleted = projDM.is_deleted;

                    uow.ProjectsRepo.Add(proj);//.saveOperation(projMaster, System.Data.Entity.EntityState.Added);
                    uow.commitTT();
                    projDM.project_syscode = proj.project_syscode;
                    od = SaveProjectModuleMembers(projDM);
                    if (od.opStatus)
                    {
                        projDM.opStatus = true;
                        projDM.opMsg = "Project created successfully!";
                    }
                    return projDM;
                }
            }
            catch (Exception ex)
            {
                Exception e = ex.ReturnActualException();
                string exMsg = e.Message;             
                Log.LogError(exMsg, "", projDM.created_by.ToString(), "PostProject", "ProjectController");
                od.opStatus = false;
                od.opMsg = "Exception Occurred! "+ exMsg;                
                od.opInnerException = e;
            }
            return od;
        }

        private OperationDetailsDTO SaveProjectModuleMembers(ProjectDM projDM)
        {
            OperationDetailsDTO od = new OperationDetailsDTO();
            using (var uow = new UnitOfWork())
            {
                if (projDM.arrReadUserSyscodes != null)
                {
                    List<ProjModUserMapping> lstReadUsers = new List<ProjModUserMapping>();

                    foreach (var arrItem in projDM.arrReadUserSyscodes)
                    {
                        ProjModUserMapping user = new ProjModUserMapping();
                        user.employee_syscode = arrItem;
                        user.role_syscode = (int)Enum_Master.UserRoleEnum.Project_User;   //Enum.GetName(typeof(TEnum), value)
                        user.project_syscode = projDM.project_syscode;
                        user.module_syscode = null;
                        user.access_read = true;
                        user.created_by = projDM.created_by;
                        lstReadUsers.Add(user);
                    }

                    List<ProjModUserMapping> ProjModReadUsersinDB = uow.ProjModUserMappingRepo
                                                                    .GetList(x => x.project_syscode.Equals(projDM.project_syscode)
                                                                                && x.role_syscode == (int)Enum_Master.UserRoleEnum.Project_User
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
                        lstDelTskUsr.ForEach(x => { x.is_deleted = true; x.is_active = false; x.modified_by = projDM.created_by; x.modified_on = DateTime.Now; });

                        uow.ProjModUserMappingRepo.UpdateRange(lstDelTskUsr);
                    }
                    else
                    {
                        lstNewReadUsr = lstReadUsers;
                    }
                    uow.ProjModUserMappingRepo.AddRange(lstNewReadUsr);
                    uow.commitTT();
                }
                if (projDM.arrWriteUserSyscodes != null)
                {
                    List<ProjModUserMapping> lstWriteUsers = new List<ProjModUserMapping>();

                    foreach (var arrItem in projDM.arrWriteUserSyscodes)
                    {
                        ProjModUserMapping user = new ProjModUserMapping();
                        user.employee_syscode = arrItem;
                        user.role_syscode = (int)Enum_Master.UserRoleEnum.Project_User;   //Enum.GetName(typeof(TEnum), value)
                        user.project_syscode = projDM.project_syscode;
                        user.module_syscode = null;
                        user.access_read = true;
                        user.access_write = true;
                        user.created_by = projDM.created_by;
                        lstWriteUsers.Add(user);
                    }

                    List<ProjModUserMapping> ProjModWriteUsersinDB = uow.ProjModUserMappingRepo
                                                                    .GetList(x => x.project_syscode.Equals(projDM.project_syscode)
                                                                                && x.role_syscode == (int)Enum_Master.UserRoleEnum.Project_User
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
                        lstDelUsr.ForEach(x => { x.is_deleted = true; x.is_active = false; x.modified_by = projDM.created_by; x.modified_on = DateTime.Now; });

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

        [HttpPost]
        public OperationDetailsDTO GetProjectByID([FromBody] ProjectDM project)
        {
            OperationDetailsDTO od = new OperationDetailsDTO();
            ProjectDM wf = project;

            if (project.project_syscode <= 0) throw new Exception("Invalid project reference, Please make sure selected project is valid.");

            try
            {
                using (var uow = new UnitOfWork())
                {
                    var projModel = uow.ProjectsRepo.GetList(x => x.is_deleted == false && x.project_syscode.Equals(project.project_syscode), x => x.lstModules)?.FirstOrDefault();
                    projModel.PageHasWriteAccess = uow.AccessControlRepo.returnProjectAccess(project.logged_in_user, projModel.project_syscode);

                    if (projModel == null)
                        throw new Exception("Some Error occured while fetching project data.");

                    projModel.lstModules = projModel.lstModules.Where(x => x.is_deleted == false).ToList();

                    wf = projModel.Map<ProjectMaster, ProjectDM>();

                    if (wf == null) throw new Exception("Some error occured while mapping ProjectMaster to ProjectDM");

                    wf.ddlData = project.ddlData; //Again replacing with original ddlData object as it was replaced by the auto mapper while mapping from the entity to domain model.

                    wf.arrReadUserSyscodes =  uow.ProjModUserMappingRepo.GetList(x => x.is_deleted == false 
                                                                                   && x.project_syscode.Equals(project.project_syscode)
                                                                                   && x.access_read
                                                                                   && x.access_write == false
                                                                                   && x.is_active
                                                                                   && x.role_syscode == (int)Enum_Master.UserRoleEnum.Project_User
                                                                                   && !x.is_deleted
                                                                                 )
                                                                        .Select(x => x.employee_syscode).ToArray();

                    wf.arrWriteUserSyscodes = uow.ProjModUserMappingRepo.GetList(x => x.is_deleted == false
                                                                                && x.project_syscode.Equals(project.project_syscode)
                                                                                && x.access_write
                                                                                && x.is_active
                                                                                && x.role_syscode == (int)Enum_Master.UserRoleEnum.Project_User
                                                                                && !x.is_deleted
                                                                               )
                                                                      .Select(x => x.employee_syscode).ToArray();
                    uow.CommonRepo.fillDDLdata(wf.ddlData);  //wf.ddlData.fillDDLdata(ComRepo.fillDDLdata);

                    wf.opStatus = true;
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

            return wf;
        }
        [HttpPut]
        public OperationDetailsDTO PutProject([FromBody] ProjectDM projDM)
        {
            ProjectMaster proj = null;

            OperationDetailsDTO od = new OperationDetailsDTO();
            if (projDM == null || projDM.project_syscode == 0)
            {
                od.opStatus = false;
                od.opMsg = "Invalid Project";
            }
            try
            {
                if (ComLibCommon.CheckDuplicateUser(projDM.arrReadUserSyscodes, projDM.arrWriteUserSyscodes))
                {
                    throw new Exception("Read Access User cannot be added as Write Access User.");
                }

                using (var uow = new UnitOfWork())
                {
                    proj = uow.ProjectsRepo.GetSingle(x => x.is_deleted == false
                                                                      && x.project_name.Equals(projDM.project_name)
                                                                      && x.project_syscode != projDM.project_syscode
                                                                      && x.group_syscode == projDM.group_syscode);
                    if (proj != null)
                    {
                        throw new Exception("Project with name " + projDM.project_name + " already exists in this Group.");
                    }

                    proj = uow.ProjectsRepo.Get(projDM.project_syscode);
                    proj.project_name = projDM.project_name;
                    proj.project_description = projDM.project_description;
                    proj.is_active = projDM.is_active;
                    proj.modified_by = projDM.modified_by;

                    uow.ProjectsRepo.Update(proj);
                    uow.commitTT();

                    od = SaveProjectModuleMembers(projDM);
                    //projDM.arrReadUserSyscodes = uow.ProjModUserMappingRepo.GetList(x => x.is_deleted == false
                    //                                                            && x.project_syscode.Equals(proj.project_syscode)
                    //                                                            && x.access_read
                    //                                                            && x.access_write == false
                    //                                                            && x.is_active
                    //                                                           )
                    //                                                  .Select(x => x.employee_syscode).ToArray();

                    //projDM.arrWriteUserSyscodes = uow.ProjModUserMappingRepo.GetList(x => x.is_deleted == false
                    //                                                            && x.project_syscode.Equals(proj.project_syscode)
                    //                                                            && x.access_write
                    //                                                            && x.is_active
                    //                                                           )
                    //                                                  .Select(x => x.employee_syscode).ToArray();
                    //uow.CommonRepo.fillDDLdata(projDM.ddlData);
                    if (od.opStatus)
                    {
                        projDM.opStatus = true;
                        projDM.opMsg = "Project updated successfully!";
                    }                   
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", projDM.created_by.ToString(), "PutProject", "ProjectController");
                projDM.opStatus = false;
                projDM.opMsg = ex.Message;
                projDM.opInnerException = ex;
            }
            return projDM;
        }


        [HttpDelete]
        public OperationDetailsDTO DeleteProject([FromBody] ProjectMaster proj)
        {
            OperationDetailsDTO od = new OperationDetailsDTO();
            if (proj == null || proj.project_syscode == 0)
            {
                od.opStatus = false;
                od.opMsg = "Invalid Project";
            }
            try
            {
                using (var uow = new UnitOfWork())
                {
                    uow.ProjectsRepo.Update(proj);//saveOperation(proj, System.Data.Entity.EntityState.Deleted); 
                    uow.commitTT();
                    od.opStatus = true;
                    od.opMsg = "Successfully deleted the project.";
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", proj.created_by.ToString(), "DeleteProject", "ProjectController");
                od.opStatus = false;
                od.opMsg = ex.Message;
                od.opInnerException = ex;
            }
            return od;
        }

        #region Project Details

        [HttpPost]
        public OperationDetailsDTO GetProjectDetails([FromBody] ProjectDM project)
        {
            OperationDetailsDTO od = new OperationDetailsDTO();
            ProjectDM projDM = project;

            if (project.project_syscode <= 0) throw new Exception("Invalid project reference, Please make sure selected project is valid.");

            try
            {
                using (var uow = new UnitOfWork())
                {
                    List<ProjectDetails> projDetailsList = uow.ProjectsRepo.GetProjectDetailsList(project.project_syscode, project.env_syscode);

             
                    if (projDetailsList == null)
                        throw new Exception("Some Error occured while fetching project data.");

                    projDM.lstProjDetails = projDetailsList;
                    projDM.ddlData = project.ddlData; 
                    uow.CommonRepo.fillDDLdata(projDM.ddlData); 
                    projDM.opStatus = true;
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

            return projDM;
        }

        
        [HttpPost]
        public ProjectMaster AddUpdateProjectDetails([FromBody] ProjectDM projDM)//IList<WorkflowLevelDetails> workflowLevels
        {
            ProjectMaster objPM = new ProjectMaster();

            if (projDM.project_syscode <= 0) throw new Exception("Invalid project reference, Please make sure selected project is valid.");

            IList<ProjectDetails> lstProjDetails = projDM.lstProjDetails;

            if (lstProjDetails.Count == 0)
            {
                objPM.opStatus = false;
                objPM.opMsg = "Project Details not found.";
                return objPM;
            }
            
            try
            {
                //--Not required--
                //var anyDuplicate = lstProjDetails.GroupBy(x => x.parameter_syscode).Any(g => g.Count() > 1);
                //if (anyDuplicate)
                //    throw new Exception("Records not updated. Duplicate Parameters found. ");

                using (var uow = new UnitOfWork())
                {
                    foreach (var detail in lstProjDetails)
                    {
                        if (detail.details_syscode > 0)
                        {
                            uow.ProjDetailsRepo.Update(detail);
                            detail.modified_by = projDM.created_by;
                            detail.modified_on = DateTime.Now;
                        }                        
                        else
                        {
                            detail.created_by = projDM.created_by;
                            uow.ProjDetailsRepo.Add(detail);
                        }                    
                    }

                    uow.commitTT();
                    objPM.opStatus = true;
                    objPM.lstProjDetails = lstProjDetails; //since this entity object is directly tracked by the context, its primary key gets updated.
                   
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", "", "AddUpdateProjectDetails", "ProjectController");
                objPM.opStatus = false;
                objPM.opMsg = ex.Message;
            }
            return objPM;
        }

        #endregion

    }
}
