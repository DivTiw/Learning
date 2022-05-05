using Common_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Library.Repository;

namespace Task_tracker_WebAPI.Areas.Reports.Controllers
{
    public class ReportsController : ApiController
    {
        [HttpPost]
        public IList<ReleaseDM> GetTaskReleaseReport(ReleaseDM relDM)
        {
            List<ReleaseDM> lst = null;
            try
            {
                using (var uow = new UnitOfWork())
                {
                    lst = uow.ReleaseInstRepo.getTaskReleaseReportList(relDM.task_syscode);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", relDM.logged_in_user.ToString(), "GetTaskReleaseReport", "ReportAPI");
            }
            return lst;
        }

        [HttpPost]
        public OperationDetailsDTO UpdateRelease([FromBody] ReleaseDM relDM)
        {          
            try
            {
                if (relDM.release_syscode <= 0)
                {
                    throw new Exception("Invalid Release.");
                }
                if (string.IsNullOrEmpty(relDM.Remarks))
                {
                    throw new Exception("Remarks cannot be left blank.");
                }               

                using (var uow = new UnitOfWork())
                {    
                    ReleaseInstructions release = uow.ReleaseInstRepo.GetSingle(x => x.is_active
                                                                                    && x.is_deleted == false 
                                                                                    && x.release_syscode == relDM.release_syscode);
                    if (release == null)
                    {
                        throw new Exception("Release not found in the database.");
                    }

                    release.is_Released = true;
                    release.Remarks = relDM.Remarks;
                    release.modified_by = relDM.logged_in_user;
                    release.modified_on = DateTime.Now;
                    uow.ReleaseInstRepo.Update(release);
                    uow.commitTT();

                   OperationDetailsDTO OD = SendReleaseEmail(relDM);

                    if (!OD.opStatus)
                    {
                        relDM.opMsg = "eMail could not be sent.";
                        relDM.opStatus = false;
                        return relDM;
                    }                  

                    relDM.opStatus = true;
                    relDM.opMsg = "Record updated sucessfully.";
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", relDM.logged_in_user.ToString(), "UpdateRelease", "Reports");
                relDM.opStatus = false;
                relDM.opMsg = "Error: " + ex.Message;
                relDM.opInnerException = ex;
            }
            return relDM;
        }

        private OperationDetailsDTO SendReleaseEmail(ReleaseDM relDM)
        {
            OperationDetailsDTO od = null;
            List<int> to_syscodes = new List<int>();
            List<int> cc_syscodes = new List<int>();
            string strMembers = string.Empty;

            try
            {
                od = new OperationDetailsDTO();
                using (var uow = new UnitOfWork())
                {

                    ReleaseDM release = uow.ReleaseInstRepo.getReleaseInstructionsByID(relDM.release_syscode);

                    //JMTask objTask = uow.TasksRepo.Get(release.task_syscode);
                    TaskDM taskDM = uow.TasksRepo.getTaskByID(release.task_syscode, relDM.logged_in_user);

                    //List<TaskUserMapping> TaskUserMappingsinDB = uow.TaskUserMappingRepo
                    //                                              .GetList(x => x.task_syscode.Equals(objTask.task_syscode)
                    //                                                          && !x.is_deleted
                    //                                                          && x.is_active)
                    //                                              ?.ToList();
                    taskDM.lstUsers.ForEach(x => { cc_syscodes.Add(x.employee_syscode); });

                    EmailTemplate temp = uow.EmailTemplateRepo.GetList(x => x.template_name.Equals("Release_Done") && x.is_active).FirstOrDefault();
                    if (temp == null)
                    {
                        throw new Exception("Email Template Not found.");
                    }
                    int template_syscode = temp.template_syscode;
                    string email_subject = temp.template_subject;
                    string email_from_display = temp.from_email_display;
                    string email_from_id = temp.from_email_id;
                    string email_body = temp.template_body;

                    string email_link = temp.link_url + ComLibCommon.Base64Encode(taskDM.task_syscode + "");

                    to_syscodes.Add(release.created_by);
                    cc_syscodes.Add(taskDM.created_by);
                    cc_syscodes.Add(taskDM.task_on_behalf);
                    cc_syscodes.Add(taskDM.task_owner);
                    cc_syscodes.Add(release.modified_by?? 0);

                    strMembers = uow.CommonRepo.getEmployeeNames(to_syscodes.ToArray());

                    email_subject = email_subject.Replace("#project_name#", taskDM.project?.project_name);
                    email_subject = email_subject.Replace("#module_name#", taskDM.module?.module_name);
                    email_subject = email_subject.Replace("#rel_ref#", release.release_ref);
                    email_subject = email_subject.Replace("#wf_level#", taskDM.level_name);
                    email_subject = email_subject.Replace("#env#", release.env.env_name);

                    email_body = email_body.Replace("#rel_ref#", release.release_ref);
                    email_body = email_body.Replace("#env#", release.env.env_name);
                    email_body = email_body.Replace("#to_emp#", strMembers);
                    email_body = email_body.Replace("#emp_name#", relDM.logged_in_user_name);
                    email_body = email_body.Replace("#project#", taskDM.project?.project_name);                    
                    email_body = email_body.Replace("#module#", taskDM.module?.module_name);                   
                    email_body = email_body.Replace("#subject#", taskDM.task_subject);                   
                    email_body = email_body.Replace("#remarks#", release.Remarks);
             
                    email_body = email_body.Replace("#url#", email_link);

                    email_from_display = email_from_display.Replace("#emp_name#", relDM.logged_in_user_name);

                    bool emailSent = uow.EmailRepo.SendEmail(release.created_by, template_syscode, email_subject, email_from_display, email_from_id, email_body, to_syscodes, cc_syscodes);
                    if (!emailSent)
                    {
                        od.opStatus = false;
                        od.opMsg = "eMail could not be sent.";
                    }
                    else
                    {
                        od.opStatus = true;
                        od.opMsg = "Email successfully sent.";
                    }                  
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", relDM.logged_in_user.ToString(), "SendReleaseEmail", "ReportsAPI");
                od.opStatus = false;
                od.opMsg = ex.Message;
            }
            return od;

        }
    }
}
