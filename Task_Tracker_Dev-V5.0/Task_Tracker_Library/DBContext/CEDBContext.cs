using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Library.DBContext
{
    public class CEDBContext : DbContext
    {
        public CEDBContext() : base("name=CommonEmailConnection")
        {
        }

        public ObjectContext ObjectContext()
        {
            return (this as IObjectContextAdapter).ObjectContext;
        }
        public DbSet<CE_vw_employee_master> vw_employee_master { get; set; }
        public virtual DbSet<common_email_outbox> common_email_outbox { get; set; }
        public virtual DbSet<common_email_outbox_master> common_email_outbox_master { get; set; }

        //public virtual int proc_save_email_to_outbox(int? user_syscode, int? template_syscode, string from_email_display, string from_email_id, string to_email_id, string cc_email_id, string subject, string body, int? created_by, DateTime? created_on, int? retry_count, string status, int? project_syscode, ObjectParameter return_value, ObjectParameter outBox_Syscode, string tempAttachment, string bcc_email_id)
        //{
        //    var user_syscodeParameter = user_syscode.HasValue ?
        //        new ObjectParameter("user_syscode", user_syscode) :
        //        new ObjectParameter("user_syscode", typeof(int));

        //    var template_syscodeParameter = template_syscode.HasValue ?
        //        new ObjectParameter("template_syscode", template_syscode) :
        //        new ObjectParameter("template_syscode", typeof(int));

        //    var from_email_displayParameter = from_email_display != null ?
        //        new ObjectParameter("from_email_display", from_email_display) :
        //        new ObjectParameter("from_email_display", typeof(string));

        //    var from_email_idParameter = from_email_id != null ?
        //        new ObjectParameter("from_email_id", from_email_id) :
        //        new ObjectParameter("from_email_id", typeof(string));

        //    var to_email_idParameter = to_email_id != null ?
        //        new ObjectParameter("to_email_id", to_email_id) :
        //        new ObjectParameter("to_email_id", typeof(string));

        //    var cc_email_idParameter = cc_email_id != null ?
        //        new ObjectParameter("cc_email_id", cc_email_id) :
        //        new ObjectParameter("cc_email_id", typeof(string));

        //    var subjectParameter = subject != null ?
        //        new ObjectParameter("subject", subject) :
        //        new ObjectParameter("subject", typeof(string));

        //    var bodyParameter = body != null ?
        //        new ObjectParameter("body", body) :
        //        new ObjectParameter("body", typeof(string));

        //    var created_byParameter = created_by.HasValue ?
        //        new ObjectParameter("created_by", created_by) :
        //        new ObjectParameter("created_by", typeof(int));

        //    var created_onParameter = created_on.HasValue ?
        //        new ObjectParameter("created_on", created_on) :
        //        new ObjectParameter("created_on", typeof(DateTime));

        //    var retry_countParameter = retry_count.HasValue ?
        //        new ObjectParameter("retry_count", retry_count) :
        //        new ObjectParameter("retry_count", typeof(int));

        //    var statusParameter = status != null ?
        //        new ObjectParameter("status", status) :
        //        new ObjectParameter("status", typeof(string));

        //    var project_syscodeParameter = project_syscode.HasValue ?
        //        new ObjectParameter("project_syscode", project_syscode) :
        //        new ObjectParameter("project_syscode", typeof(int));

        //    var tempAttachmentParameter = tempAttachment != null ?
        //        new ObjectParameter("tempAttachment", tempAttachment) :
        //        new ObjectParameter("tempAttachment", typeof(string));

        //    var bcc_email_idParameter = bcc_email_id != null ?
        //        new ObjectParameter("bcc_email_id", bcc_email_id) :
        //        new ObjectParameter("bcc_email_id", typeof(string));

        //    return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("proc_save_email_to_outbox", user_syscodeParameter, template_syscodeParameter, from_email_displayParameter, from_email_idParameter, to_email_idParameter, cc_email_idParameter, subjectParameter, bodyParameter, created_byParameter, created_onParameter, retry_countParameter, statusParameter, project_syscodeParameter, return_value, outBox_Syscode, tempAttachmentParameter, bcc_email_idParameter);
        //}

        public virtual string proc_save_email_to_outbox(int? usersyscode, int? templatesyscode, string fromEmail_display, string fromEmail_id, string toEmail_id, string ccEmail_id, string emailSubject, string emailBody, int? createdBy, DateTime? createdOn, int? retry_cnt, string emailStatus, int? proj_syscode, string tempAttachment, string bcc_email_id)
        {
            SqlParameter @user_syscodeParameter = new SqlParameter()
            {
                ParameterName = "@user_syscode",
                DbType = DbType.Int32,
                Value = usersyscode
            };

            SqlParameter @template_syscodeParameter = new SqlParameter()
            {
                ParameterName = "@template_syscode",
                DbType = DbType.Int32,
                Value = templatesyscode
            };

            SqlParameter @from_email_displayParameter = new SqlParameter()
            {
                ParameterName = "@from_email_display",
                DbType = DbType.String,
                Value = fromEmail_display
            };

            SqlParameter @from_email_idParameter = new SqlParameter()
            {
                ParameterName = "@from_email_id",
                DbType = DbType.String,
                Value = fromEmail_id
            };

            SqlParameter @to_email_idParameter = new SqlParameter()
            {
                ParameterName = "@to_email_id",
                DbType = DbType.String,
                Value = toEmail_id
            };

            SqlParameter @cc_email_idParameter = new SqlParameter()
            {
                ParameterName = "@cc_email_id",
                DbType = DbType.String,
                Value = ccEmail_id
            };

            SqlParameter @subjectParameter = new SqlParameter()
            {
                ParameterName = "@subject",
                DbType = DbType.String,
                Value = emailSubject
            };

            SqlParameter @bodyParameter = new SqlParameter()
            {
                ParameterName = "@body",
                DbType = DbType.String,
                Value = emailBody
            };

            SqlParameter @created_byParameter = new SqlParameter()
            {
                ParameterName = "@created_by",
                DbType = DbType.Int32,
                Value = createdBy
            };


            SqlParameter @created_onParameter = new SqlParameter()
            {
                ParameterName = "@created_on",
                DbType = DbType.DateTime,
                Value = createdOn
            };

            SqlParameter @retry_countParameter = new SqlParameter()
            {
                ParameterName = "@retry_count",
                DbType = DbType.Int32,
                Value = retry_cnt
            };

            SqlParameter @statusParameter = new SqlParameter()
            {
                ParameterName = "@status",
                DbType = DbType.String,
                Value = emailStatus
            };

            SqlParameter @project_syscodeParameter = new SqlParameter()
            {
                ParameterName = "@project_syscode",
                DbType = DbType.Int32,
                Value = proj_syscode
            };

            SqlParameter return_valueParameter = new SqlParameter()
            {
                ParameterName = "@return_value",
                DbType = DbType.String,
                Direction = ParameterDirection.Output,
                Size = 1000
            };

            SqlParameter @OutBox_SyscodeParameter = new SqlParameter()
            {
                ParameterName = "@OutBox_Syscode",
                DbType = DbType.Int32,
                Direction = ParameterDirection.Output
            };

            SqlParameter @tempAttachmentParameter = new SqlParameter()
            {
                ParameterName = "@tempAttachment",
                DbType = DbType.String,
                Value = tempAttachment
            };

            object[] parameters = new object[] { @user_syscodeParameter, @template_syscodeParameter, @from_email_displayParameter, @from_email_idParameter,
                                                 @to_email_idParameter, @cc_email_idParameter, @subjectParameter, @bodyParameter, @created_byParameter,
                                                 @created_onParameter, @retry_countParameter, @statusParameter, @project_syscodeParameter,return_valueParameter
                                                 , @OutBox_SyscodeParameter, @tempAttachmentParameter/*, bcc_email_idParameter*/  };

            using (var ctx = new CEDBContext())
            {
                var result = ctx.Database.ExecuteSqlCommand("proc_save_email_to_outbox @user_syscode, @template_syscode, @from_email_display,@from_email_id, @to_email_id, @cc_email_id, @subject, @body, @created_by, @created_on, @retry_count, @status, @project_syscode, @return_value OUT, @OutBox_Syscode OUT, @tempAttachment", parameters);
                return return_valueParameter.Value.ToString();
            }            
        }
    }

    public class common_email_outbox
    {
        [Key]
        public int outbox_syscode { get; set; }
        public int outbox_master_syscode { get; set; }
        public string to_email_id { get; set; }
        public string cc_email_id { get; set; }
        public string bcc_email_id { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public int? created_by { get; set; }
        public int? created_for { get; set; }
        public DateTime created_on { get; set; }
        public int? retry_count { get; set; }
        public bool status { get; set; }
        public bool? error_flag { get; set; }
        public string error_desc { get; set; }
    }
    public class common_email_outbox_master
    {
        [Key]
        public int outbox_master_syscode { get; set; }
        public string from_email_display { get; set; }
        public string from_email_id { get; set; }
        public int template_syscode { get; set; }
        public int project_syscode { get; set; }
        public DateTime created_on { get; set; }
    }
}
