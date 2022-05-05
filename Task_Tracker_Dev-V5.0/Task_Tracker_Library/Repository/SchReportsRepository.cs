using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_Library.Repository
{
    public class SchReportsRepository : TTBaseRepository<JMTask>
    {
        public SchReportsRepository(TTDBContext context) : base(context)
        {
        }

        public GroupReportDM getReportDetails(ReportsEnum aRpt, int aGrpSyscode)
        {
            GroupReportDM RptDtls = new GroupReportDM();
            RptDtls = (from grm in context.GroupReportMaster
                       where grm.report_code == aRpt.ToString() && grm.group_syscode == aGrpSyscode
                       select new GroupReportDM
                       {
                           template_syscode = grm.template_syscode,
                           lstReportRecipients = context.GroupReportRecipient.Where(x => x.grp_rpt_syscode == grm.grp_rpt_syscode)
                           .Join(context.vw_employee_master, grpRcpnt=> grpRcpnt.employee_syscode, emp=> emp.employee_syscode,
                               (grpRcpnt, emp)=> new GroupReportRecipientDM { 
                                employee_syscode = grpRcpnt.employee_syscode,
                                Recipient_Name = emp.employee_name,
                                Recipient_Email = emp.email_id,
                                is_cc = grpRcpnt.is_cc
                               }).Select(x=> x).ToList()
                       }).FirstOrDefault() ;

            if (RptDtls != null)
            {
                var toArr = RptDtls.lstReportRecipients.Where(x => !x.is_cc).Select(x=> new { x.Recipient_Email, x.Recipient_Name })?.ToList();
                if (toArr != null && toArr.Count > 0)
                {
                    RptDtls.toNames = string.Join(",", toArr.Select(x => x.Recipient_Name).ToArray());
                    RptDtls.toEmails = string.Join(",", toArr.Select(x => x.Recipient_Email).ToArray()); 
                }

                var ccArr = RptDtls.lstReportRecipients.Where(x => x.is_cc).Select(x => new { x.Recipient_Email, x.Recipient_Name })?.ToList();
                if (ccArr != null && ccArr.Count > 0)
                {
                    RptDtls.ccEmails = string.Join(",", toArr.Select(x => x.Recipient_Name).ToArray()); 
                }
            }
            return RptDtls;
        }

        public IList<GroupMember_TaskDtlsDTO> proc_get_GroupMember_TaskDtls()
        {
            IList<GroupMember_TaskDtlsDTO> lstGroupMemTskDtls = null;

            lstGroupMemTskDtls = context.Database.SqlQuery<GroupMember_TaskDtlsDTO>("EXEC [dbo].[proc_get_GroupMember_TaskDtls]").ToList();

            return lstGroupMemTskDtls;
        }        

        public DataTable proc_LMS_GetDailyAttendanceReport(int emp_syscode, DateTime RptStartDate, DateTime RptEndDate)
        {
           
            SqlParameter @employee_syscode = new SqlParameter()
            {
                ParameterName = "@employee_syscode",
                DbType = DbType.Int32,
                Value = DBNull.Value
            }; SqlParameter @PageCalledFor = new SqlParameter()
            {
                ParameterName = "@PageCalledFor",
                DbType = DbType.String,
                Value = DBNull.Value
            }; SqlParameter @PageTypeFor = new SqlParameter()
            {
                ParameterName = "@PageTypeFor",
                DbType = DbType.String,
                Value = DBNull.Value
            };

            SqlParameter @search_employee_syscodes = new SqlParameter()
            {
                ParameterName = "@search_employee_syscodes",
                DbType = DbType.Int32,
                Value = emp_syscode
            };
            SqlParameter @search_first_date = new SqlParameter()
            {
                ParameterName = "@search_first_date",
                DbType = DbType.DateTime,
                Value = RptStartDate
            };
            SqlParameter @search_last_date = new SqlParameter()
            {
                ParameterName = "@search_last_date",
                DbType = DbType.DateTime,
                Value = RptEndDate
            };

            SqlParameter @search_company_syscode = new SqlParameter()
            {
                ParameterName = "@search_company_syscode",
                DbType = DbType.Int32,
                Value = DBNull.Value
            };

            SqlParameter @search_department_syscode = new SqlParameter()
            {
                ParameterName = "@search_department_syscode",
                DbType = DbType.Int32,
                Value = DBNull.Value
            };

            SqlParameter @search_location_syscode = new SqlParameter()
            {
                ParameterName = "@search_location_syscode",
                DbType = DbType.Int32,
                Value = DBNull.Value
            };

            SqlParameter @search_sub_location_syscode = new SqlParameter()
            {
                ParameterName = "@search_sub_location_syscode",
                DbType = DbType.Int32,
                Value = DBNull.Value
            };

            SqlParameter @return_value = new SqlParameter()
            {
                ParameterName = "@return_value",
                Direction = ParameterDirection.Output,
                DbType = DbType.String,
                Size = 1000
            };

            object[] parameters = new object[] { @employee_syscode, @PageCalledFor, @PageTypeFor, @search_employee_syscodes, @search_first_date, @search_last_date, @search_company_syscode, @search_department_syscode, @search_location_syscode, @search_sub_location_syscode, @return_value };

            SqlConnection con = (SqlConnection)context.Database.Connection;
            SqlCommand cmd = new SqlCommand("proc_LMS_GetDailyAttendanceReport", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddRange(parameters);

            DataTable dt = new DataTable();

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            return dt;            
        }        
    }
}
