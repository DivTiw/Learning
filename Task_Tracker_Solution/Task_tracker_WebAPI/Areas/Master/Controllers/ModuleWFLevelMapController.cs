using Common_Components;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ModuleWFLevelMapController : BaseAPIController
    {
        [HttpPost]
        public ModuleWFLevelMapDM GetModulesListByProject(ProjectMaster pm)
        {
            ModuleWFLevelMapDM dm = null;
            //ModuleWFMapRepository<ModuleMaster> Repo = new ModuleWFMapRepository<ModuleMaster>(new TTDBContext());
            try
            {
                using (var uow = new UnitOfWork())
                {
                    dm = new ModuleWFLevelMapDM();
                    dm.ddlData = uow.CommonRepo.fillDDLdata(dm.ddlData); // Fill Project List
                    IQueryable<ModuleMaster> lstmod = uow.ModuleRepo.GetList(x => x.project_syscode == pm.project_syscode && x.is_active && !x.is_deleted).AsQueryable();
                    dm.ddlData.Data.Add(DBTableNameEnums.ModuleMaster, lstmod.Select(x => new SelectItemDTO
                    {
                        Text = x.module_name,
                        Value = x.module_syscode
                    }).ToList());
                    dm.opStatus = true;
                }

            }
            catch (Exception ex)
            {
                dm.opStatus = false;
                dm.opMsg = ex.Message;
                dm.opInnerException = ex;
                Log.LogError(ex.Message, "", null, "GetModulesListByProject", "ModuleWFLevelMapController");
            }
            return dm;
        }

        [HttpPost]
        public ModuleWFLevelMapDM GetLevelTaskUsersList(ModuleWFLevelMapDM wfLTU)
        {
            try
            {
                using (var uow = new UnitOfWork())
                {
                    wfLTU.ddlData = uow.CommonRepo.fillDDLdata(wfLTU.ddlData);

                    //var lst = uow.ModuleWFMapRepo.GetWFLevels_UsersAndWeightage(wfLTU.module_syscode);
                    var lst = uow.ModuleWFMapRepo.GetModuleLevelDetails(wfLTU.module_syscode);
                    if (lst != null)
                    {
                        wfLTU.lstLevelTaskUsers = lst;
                    }
                    wfLTU.opStatus = true;
                }
            }
            catch (Exception ex)
            {
                wfLTU.opStatus = false;
                wfLTU.opMsg = ex.Message;
                wfLTU.opInnerException = ex;
                Log.LogError(ex.Message, "", null, "GetLevelTaskUsersList", "ModuleWFLevelMapController");
            }
            return wfLTU;
        }


        [HttpPost]
        public ModuleWFLevelMapDM AddWFLevelMapping(ModuleWFLevelMapDM MapDM)
        {
            string email_rows = string.Empty;
            List<int> to_syscodes = new List<int>();

            if (MapDM.lstLevelTaskUsers.Count <= 0)
                throw new ArgumentNullException("lstLevelTaskUsers", "List of Workflow Levels can not be empty.");
            try
            {
                OperationDetailsDTO od = new OperationDetailsDTO();
                using (var uow = new UnitOfWork())
                {                   
                    uow.CommonRepo.fillDDLdata(MapDM.ddlData);                  

                    foreach (var level in MapDM.lstLevelTaskUsers)
                    {
                        ModuleLevelDetail objDetail = null;
                    
                        //if (level.arrUserSyscodes == null && level.weightage <= 0)
                        //{
                        //    continue;
                        //}

                        if (level.details_syscode > 0)
                        {
                            objDetail = uow.ModuleLevelDetailRepo.Get(level.details_syscode??0);
                            objDetail.weightage = level.weightage ?? 0;
                            objDetail.modified_by = MapDM.created_by;
                            objDetail.modified_on = DateTime.Now;
                          
                            uow.ModuleLevelDetailRepo.Update(objDetail);
                        }
                        else
                        {

                            objDetail = new ModuleLevelDetail();
                            objDetail.created_by = MapDM.created_by;
                            objDetail.created_on = DateTime.Now;
                            objDetail.is_active = true;
                            objDetail.level_syscode = level.level_syscode;
                            objDetail.module_syscode = MapDM.module_syscode;
                            objDetail.weightage = level.weightage ?? 0;                            

                            uow.ModuleLevelDetailRepo.Add(objDetail);
                        }

                        uow.commitTT();

                        if (level.arrUserSyscodes != null)
                        {
                            List<ModuleLevelUserMapping> lvlUsers = new List<ModuleLevelUserMapping>();

                            foreach (var arrItem in level.arrUserSyscodes)
                            {
                                ModuleLevelUserMapping user = new ModuleLevelUserMapping();
                                user.employee_syscode = arrItem;
                                user.details_syscode = objDetail.details_syscode;                     
                                user.created_by = MapDM.created_by;
                                lvlUsers.Add(user);
                            }

                            List<ModuleLevelUserMapping> UserMappingsinDB = uow.ModuleLevelUserMappingRepo
                                                                            .GetList(x => x.details_syscode.Equals(objDetail.details_syscode)
                                                                                        && !x.is_deleted
                                                                                        && x.is_active)
                                                                            ?.ToList();
                            List<ModuleLevelUserMapping> lstNewTskUsr = null;
                            if (UserMappingsinDB != null && UserMappingsinDB.Count > 0)
                            {
                                //List of users removed from mapping from the UI and needs to be deleted and made is active false.
                                List<ModuleLevelUserMapping> lstDelTskUsr = UserMappingsinDB.Where(x => !lvlUsers.Any(y => y.employee_syscode.Equals(x.employee_syscode))).ToList();
                                //List of users new and not present in mapping table.
                                lstNewTskUsr = lvlUsers.Where(x => !UserMappingsinDB.Any(y => y.employee_syscode.Equals(x.employee_syscode))).ToList();

                                //Process for deleting the mappings which are not present in the UI. Changing the property value of list to make it deleted.
                                lstDelTskUsr.ForEach(x => { x.is_deleted = true; x.is_active = false; x.modified_by = MapDM.created_by; x.modified_on = DateTime.Now; });

                                uow.ModuleLevelUserMappingRepo.UpdateRange(lstDelTskUsr);
                            }
                            else
                            {
                                lstNewTskUsr = lvlUsers;
                            }
                            uow.ModuleLevelUserMappingRepo.AddRange(lstNewTskUsr);
                        }                       

                        uow.commitTT();
                        MapDM.opStatus = true;

                        //Update syscode to store in hidden field in view after save
                        MapDM.lstLevelTaskUsers.Where(x => x.level_syscode == level.level_syscode).First().task_syscode = objDetail.details_syscode;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                MapDM.opStatus = false;
                MapDM.opMsg = ex.Message;
                MapDM.opInnerException = ex;
                Log.LogError(ex.Message, "", null, "AddWFLevelMapping", "ModuleWFLevelMapController");
            }
            return MapDM;
        }


      
        //Old Code -----
        //[HttpPost]
        //public ModuleWFLevelMapDM AddWFLevelMapping(ModuleWFLevelMapDM MapDM)
        //{
        //    string email_rows = string.Empty;
        //    string module_name = "";
        //    string project_name = "";
        //    bool isTaskCreated = false;
        //    List<int> to_syscodes = new List<int>();

        //    if (MapDM.lstLevelTaskUsers.Count <= 0)
        //        throw new ArgumentNullException("lstLevelTaskUsers", "List of Workflow Levels can not be empty.");
        //    try
        //    {                
        //        OperationDetailsDTO od = new OperationDetailsDTO();
        //        using (var uow = new UnitOfWork())
        //        {
        //            int linkCounter = 0;
        //            uow.CommonRepo.fillDDLdata(MapDM.ddlData);

        //            #region "Get Email Template"
        //            //Email NOT to be sent here hence commented out----

        //            //Email_Template temp = uow.EmailTemplateRepo.GetList(x => x.template_name.Equals("WF_Task_Created") && x.is_active).FirstOrDefault();
        //            //if (temp == null)
        //            //{
        //            //    throw new Exception("Email Template Not found.");
        //            //}


        //            //int template_syscode = temp.template_syscode;
        //            //string email_subject = temp.template_subject;
        //            //string email_from_display = temp.from_email_display;
        //            //string email_from_id = temp.from_email_id;
        //            //string email_body = temp.template_body;

        //            #endregion

        //            foreach (var level in MapDM.lstLevelTaskUsers)
        //            {
        //                JMTask objTask = null;
        //                string task_ref = "REF_ROOT_" + String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000);

        //                if (level.arrUserSyscodes == null && level.weightage <= 0)
        //                {
        //                    continue;
        //                }

        //                if (level.task_syscode > 0)
        //                {
        //                    isTaskCreated = false;
        //                    objTask = uow.TasksRepo.Get(level.task_syscode ?? 0);
        //                    objTask.weightage = level.weightage ?? 0;
        //                    objTask.modified_by = MapDM.created_by;
        //                    objTask.modified_on = DateTime.Now;
        //                    objTask.task_syscode = level.task_syscode ?? 0;

        //                    //uow.TasksRepo.Property("FirstName").IsModified = true;
        //                    uow.TasksRepo.Update(objTask);
        //                }
        //                else
        //                {
        //                    isTaskCreated = true;
        //                    objTask = new JMTask();
        //                    objTask.created_by = MapDM.created_by;
        //                    objTask.created_on = DateTime.Now;
        //                    objTask.task_syscode = level.task_syscode ?? 0;
        //                    objTask.is_active = true;
        //                    objTask.level_syscode = level.level_syscode;
        //                    objTask.module_syscode = MapDM.module_syscode;
        //                    objTask.task_reference = task_ref;
        //                    objTask.task_subject = level.level_name;
        //                    objTask.weightage = level.weightage ?? 0;
        //                    objTask.task_status_syscode = uow.TaskStatusRepo.GetSingle(x => x.status_name == Enum_Master.StatusEnum.Open.ToString()).status_syscode;
        //                    objTask.task_priority_syscode = uow.TaskPriorityRepo.GetSingle(x => x.priority_name == Enum_Master.PriorityEnum.Low.ToString()).priority_syscode;
        //                    objTask.category_syscode = null;

        //                    uow.TasksRepo.Add(objTask);//saveOperation(task, System.Data.Entity.EntityState.Added);
        //                }

        //                uow.commitTT();

        //                if (level.arrUserSyscodes != null)
        //                {
        //                    List<TaskUserMapping> taskUsers = new List<TaskUserMapping>();

        //                    foreach (var arrItem in level.arrUserSyscodes)
        //                    {
        //                        TaskUserMapping user = new TaskUserMapping();
        //                        user.employee_syscode = arrItem;//Convert.ToInt32(arrItem);                            
        //                        user.user_role_syscode = (int)Enum_Master.UserRoleEnum.Created_For;   //Enum.GetName(typeof(TEnum), value)
        //                        user.task_syscode = objTask.task_syscode;
        //                        user.trail_syscode = null;
        //                        user.created_by = MapDM.created_by;
        //                        taskUsers.Add(user);
        //                    }

        //                    List<TaskUserMapping> TaskUserMappingsinDB = uow.TaskUserMappingRepo
        //                                                                    .GetList(x => x.task_syscode.Equals(objTask.task_syscode)
        //                                                                                && !x.is_deleted
        //                                                                                && x.is_active)
        //                                                                    ?.ToList();
        //                    List<TaskUserMapping> lstNewTskUsr = null;
        //                    if (TaskUserMappingsinDB != null && TaskUserMappingsinDB.Count > 0)
        //                    {
        //                        //List of users removed from mapping from the UI and needs to be deleted and made is active false.
        //                        List<TaskUserMapping> lstDelTskUsr = TaskUserMappingsinDB.Where(x => !taskUsers.Any(y => y.employee_syscode.Equals(x.employee_syscode))).ToList();
        //                        //List of users new and not present in mapping table.
        //                        lstNewTskUsr = taskUsers.Where(x => !TaskUserMappingsinDB.Any(y => y.employee_syscode.Equals(x.employee_syscode))).ToList();

        //                        //Process for deleting the mappings which are not present in the UI. Changing the property value of list to make it deleted.
        //                        lstDelTskUsr.ForEach(x => { x.is_deleted = true; x.is_active = false; x.modified_by = MapDM.created_by; x.modified_on = DateTime.Now; });

        //                        uow.TaskUserMappingRepo.UpdateRange(lstDelTskUsr);
        //                    }
        //                    else
        //                    {
        //                        lstNewTskUsr = taskUsers;
        //                    }
        //                    uow.TaskUserMappingRepo.AddRange(lstNewTskUsr); 
        //                }

        //                string desc = (level.task_syscode > 0 ? Enum_Master.ActivityEnum.Updated : Enum_Master.ActivityEnum.Created).ToString();
        //                //Add Task Trail
        //                TaskTrail tt = new TaskTrail();
        //                tt.created_by = MapDM.created_by;
        //                tt.created_on = DateTime.Now;
        //                tt.task_syscode = objTask.task_syscode;
        //                tt.activity_syscode = level.task_syscode > 0 ? (int)Enum_Master.ActivityEnum.Updated : (int)Enum_Master.ActivityEnum.Created;
        //                tt.trail_description = MapDM.created_by_name + " " + desc + " Task On " +DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        //                uow.TaskTrailRepo.Add(tt);

        //                uow.commitTT();
        //                MapDM.opStatus = true;

        //                //Update task syscode to store in hidden field in view after save
        //                MapDM.lstLevelTaskUsers.Where(x => x.level_syscode == level.level_syscode).First().task_syscode = objTask.task_syscode;

        //                //Email NOT to be sent here hence commented out----
        //                //if (isTaskCreated)
        //                //{
        //                //    TaskDM tdm = uow.TasksRepo.getTaskByID(objTask.task_syscode);
        //                //    string strMembers = uow.CommonRepo.getEmployeeNames(level.arrUserSyscodes);

        //                //    module_name = tdm.module.module_name;
        //                //    project_name = tdm.project.project_name;

        //                //    to_syscodes.AddRange(level.arrUserSyscodes);                        
        //                //    to_syscodes.Add(MapDM.created_by);                       
        //                //    to_syscodes.RemoveAll(item => item == 0);

        //                //    string email_link = ""; //Do not change the email link code for JWT token to work
        //                //    if (linkCounter == 0)
        //                //    {
        //                //        email_link = temp.link_url + "?returnValue=Tasks/Task/ViewTask/" + ComLibCommon.Base64Encode(objTask.task_syscode + "");
        //                //    }
        //                //    else
        //                //    {
        //                //        email_link = temp.link_url + "?returnValue" + linkCounter + "=Tasks/Task/ViewTask/" + ComLibCommon.Base64Encode(objTask.task_syscode + "");
        //                //    }
        //                //    email_rows += "<tr><td><a href=\"" + email_link + "\">" + task_ref + "</a></td><td>" + objTask.task_subject +
        //                //                  "</td><td>" + strMembers + "</td></tr>";
        //                //    linkCounter = linkCounter + 1;                          
        //                //}
        //            }
        //            #region "Send eMail"
        //            //Email NOT to be sent here hence commented out----
        //            //if (!string.IsNullOrEmpty(email_rows))
        //            //{
        //            //    email_body = email_body.Replace("#emp_name#", MapDM.created_by_name);
        //            //    email_body = email_body.Replace("#module_name#", module_name);
        //            //    email_body = email_body.Replace("#project_name#", project_name);                       
        //            //    email_body = email_body.Replace("#rows#", email_rows);

        //            //    bool emailSent = uow.EmailRepo.SendEmail(MapDM.created_by, template_syscode, email_subject, email_from_display, email_from_id, email_body, to_syscodes);
        //            //    if (!emailSent)
        //            //    {
        //            //        MapDM.opMsg = "eMail could not be sent.";
        //            //    } 
        //            //}
        //            #endregion
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MapDM.opStatus = false;
        //        MapDM.opMsg = ex.Message;
        //        MapDM.opInnerException = ex;
        //        Log.LogError(ex.Message, "", null, "AddWFLevelMapping", "ModuleWFLevelMapController");
        //    }
        //    return MapDM;
        //}
    }
}
