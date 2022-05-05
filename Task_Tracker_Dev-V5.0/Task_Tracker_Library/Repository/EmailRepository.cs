using System;
using System.Collections.Generic;
using Task_Tracker_Library.DBContext;
using System.Linq;
using System.Configuration;
using Common_Components;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_CommonLibrary.DomainModels;

namespace Task_Tracker_Library.Repository
{
    public class EmailRepository
    {
        protected CEDBContext ceContext { get; }
        protected TTDBContext ttContext { get; }

        public EmailRepository(CEDBContext _context)
        {
            ceContext = _context;
        }
        public EmailRepository(CEDBContext _ceContext, TTDBContext _ttContext) : this(_ceContext)
        {
            ttContext = _ttContext;
        }

        public EmailTemplateDM GetEmailTemplate(Email_Enums.Email_Type _email_Type, Enum_Master.TaskType? _task_type = null, int? task_Status_sys = null)
        {
            EmailTemplateDM tmplt = null;

            string email_Type = _email_Type.ToString();
            string task_type = _task_type.GetEnumMemberVal();

            tmplt = (from tmp in ttContext.EmailTemplate
                     join emailDef in ttContext.EmailDefinitions on new { col1 = tmp.template_syscode, col2 = tmp.is_active, col4 = true, col5 = false } equals new { col1 = emailDef.template_syscode, col2 = true, col4 = emailDef.is_active, col5 = emailDef.is_deleted }
                     join emailTyp in ttContext.EmailTypeMaster on new { col1 = emailDef.email_type_syscode, col2 = email_Type } equals new { col1 = emailTyp.email_type_syscode, col2 = emailTyp.code }
                     join tskTyp in ttContext.TaskTypeMaster on emailDef.task_type_syscode equals tskTyp.task_type_syscode into taskTyOuter
                     from tskT in taskTyOuter.DefaultIfEmpty()
                     where emailDef.status_syscode == task_Status_sys && tskT.code == task_type
                     select new EmailTemplateDM
                     {
                         Email_Def_Syscode = emailDef.definition_syscode,
                         template_syscode = tmp.template_syscode,
                         from_email_display = tmp.from_email_display,
                         from_email_id = tmp.from_email_id,
                         link_url = tmp.link_url,
                         str_To_CatCodes = emailDef.to_recipients,
                         str_Cc_CatCodes = emailDef.cc_recipients,
                         template_name = tmp.template_name,
                         template_subject = tmp.template_subject,
                         template_header = tmp.template_header,
                         template_body = tmp.template_body,
                         template_footer = tmp.template_footer
                     }).FirstOrDefault();

            return tmplt;
        }

        public bool SendEmail(int? userSyscode, int template_syscode, string subject, string displayName, string fromID, string body, IEnumerable<int> toEmployeeID, IEnumerable<int> ccEmployeeID = null, IEnumerable<int> bccEmployeeID = null)
        {
            IEnumerable<string> toEmailID = new List<string>();
            IEnumerable<string> ccEmailID = new List<string>();

            if (toEmployeeID != null)
            {
                toEmployeeID = toEmployeeID.Distinct();
                toEmailID = (from vw in ceContext.vw_employee_master
                             where toEmployeeID.Contains(vw.employee_syscode ?? 0)
                             select vw.email_id
                                                );
            }
            else
                return false;

            if (ccEmployeeID != null)
            {
                ccEmployeeID = ccEmployeeID.Distinct();
                ccEmailID = (from vw in ceContext.vw_employee_master
                             where ccEmployeeID.Contains(vw.employee_syscode ?? 0)
                             select vw.email_id
                                                );
            }

            if (toEmailID.Count() > 0)
            {
                string strToEmails = string.Join(", ", toEmailID);
                string strCCEmails = string.Empty;
                if (ccEmailID != null && ccEmailID.Count() > 0)
                {
                    strCCEmails = string.Join(", ", ccEmailID);
                }

                string result = ceContext.proc_save_email_to_outbox(userSyscode, template_syscode, displayName, fromID, strToEmails, strCCEmails, subject, body, userSyscode, DateTime.Now, 0, "0", ComLibCommon.ProjectSyscode, "", null);

                if (result.Equals("#SUCCESS#"))
                    return true;
                else
                    return false;
            }
            else
                return false;

        }
        public bool SendEmail(int userSyscode, int template_syscode, string displayName, string fromID, string strToEmails, string strCCEmails, string subject, string body, string attachment)
        {
            string result = ceContext.proc_save_email_to_outbox(userSyscode, template_syscode, displayName, fromID, strToEmails, strCCEmails, subject, body, userSyscode, DateTime.Now, 0, "0", ComLibCommon.ProjectSyscode, attachment, null);

            if (result.Equals("#SUCCESS#"))
                return true;
            else
                return false;
        }
    }
}
