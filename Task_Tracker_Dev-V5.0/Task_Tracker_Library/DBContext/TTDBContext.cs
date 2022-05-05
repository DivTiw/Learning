using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_Library.Interface;

namespace Task_Tracker_Library.Repository
{
    public class TTDBContext : DbContext, ITTDBContext
    {

        public TTDBContext() : base("name=TaskTrackerConnection")
        {
        }

        public ObjectContext ObjectContext()
        {
            return (this as IObjectContextAdapter).ObjectContext;
        }

        #region Tables
        public DbSet<JMTask> Tasks { get; set; }
        public DbSet<TaskAccessGroupMaster> AccessGrpMaster { get; set; }
        public DbSet<TaskAccessGroupMenuDetails> AccessGrpMnuDtl { get; set; }
        public DbSet<TaskActivityMaster> TaskActivityMaster { get; set; }
        public DbSet<TaskAttachment> TaskAttachment { get; set; }
        public DbSet<TaskCategoryContact> TaskCategoryContact { get; set; }
        public DbSet<CategoryMaster> CategoryMaster { get; set; }
        public DbSet<TaskLoginHistory> TaskLoginHistory { get; set; }
        public DbSet<TaskPriorityMaster> TaskPriorityMaster { get; set; }
        public DbSet<TaskStatusMaster> TaskStatusMaster { get; set; }
        public DbSet<TaskTrail> TaskTrail { get; set; }
        public DbSet<TaskTrailDetails> TaskTrailDetail { get; set; }
        public DbSet<TaskUserMapping> TaskUserMapping { get; set; }
        public DbSet<TaskUserRoleMaster> TaskUserRoleMaster { get; set; }
        public DbSet<ProjectMaster> ProjectMaster { get; set; }
        public DbSet<ModuleMaster> ModuleMaster { get; set; }
        public DbSet<WorkflowMaster> WorkflowMaster { get; set; }
        public DbSet<ProgressMaster> ProgressMaster { get; set; }
        public DbSet<WorkflowLevelDetails> WorkflowLevelDetails { get; set; }
        public DbSet<TaskUserRecord> TaskUserRecord { get; set; }

        public DbSet<TT_vw_employee_master> vw_employee_master { get; set; }
        public DbSet<vw_department_master> vw_department_master { get; set; }

        public DbSet<EmailTemplate> EmailTemplate { get; set; }

        public DbSet<GroupMaster> GroupMaster { get; set; }
        public DbSet<GroupMember> GroupMember { get; set; }

        public DbSet<ProjModUserMapping> ProjModUserMapping { get; set; }

        public DbSet<ModuleLevelDetail> ModuleLevelDetail { get; set; }

        public DbSet<ModuleLevelUserMapping> ModuleLevelUserMapping { get; set; }
        
        public DbSet<TaskTypeMaster> TaskTypeMaster { get; set; }

        public DbSet<EmailTypeMaster> EmailTypeMaster { get; set; }

        public DbSet<EmailRecipientMaster> EmailRecipientMaster { get; set; }

        public DbSet<EmailDefinitions> EmailDefinitions { get; set; }

        public DbSet<EnvironmentMaster> EnvironmentMaster { get; set; }

        public DbSet<ProjectParameterMaster> ProjectParameterMaster { get; set; }

        public DbSet<ProjectDetails> ProjectDetails { get; set; }

        public DbSet<ReleaseInstructions> ReleaseInstructions { get; set; }

        public DbSet<ReleaseDetails> ReleaseDetails { get; set; }
        public DbSet<GroupReportMaster> GroupReportMaster { get; set; }
        public DbSet<GroupReportRecipients> GroupReportRecipient { get; set; }

        #endregion

        #region SPs And Funtions
        public virtual ObjectResult<LoginUser> AuthoriseUser(string logonName, ObjectParameter status, ObjectParameter return_value)
        {
            var windows_login_idParameter = logonName != null ?
                new ObjectParameter("Windows_login_id", logonName) :
                new ObjectParameter("Windows_login_id", typeof(string));

            return ObjectContext().ExecuteFunction<LoginUser>("proc_authorise_user", windows_login_idParameter, status, return_value);
        }

        public virtual ObjectResult<Menu> GetAllMenus()
        {
            return ObjectContext().ExecuteFunction<Menu>("proc_get_All_menus");
        }

        public virtual ObjectResult<GroupMember_TaskDtlsDTO> proc_get_GroupMember_TaskDtls()
        {
            return ObjectContext().ExecuteFunction<GroupMember_TaskDtlsDTO>("proc_get_GroupMember_TaskDtls");
        }
        #endregion
    }
}
