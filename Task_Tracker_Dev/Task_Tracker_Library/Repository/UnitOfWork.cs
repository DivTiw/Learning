using System;
using System.Diagnostics;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_Library.DBContext;
using Task_Tracker_Library.Interface;

namespace Task_Tracker_Library.Repository
{
    public class UnitOfWork : IDisposable
    {
        private TTDBContext _TTContext = null;
        private CEDBContext _CEContext = null;

        #region Task Tracker Repository Declaration
        public ICommonRepository CommonRepo { get; }
        public LoginRepository LoginRepo { get; }
        public MasterRepository<ProgressMaster> ProgressRepo { get; set; }
        public ProjectRepository ProjectsRepo { get; }
        public TaskRepository TasksRepo { get; }
        public ModuleWFMapRepository ModuleWFMapRepo { get; }
        public ModuleRepository ModuleRepo { get; }
        public MasterRepository<WorkflowMaster> WorkflowRepo { get; }
        public MasterRepository<WorkflowLevelDetails> WorkflowLevelRepo { get; }

        public MasterRepository<TaskStatusMaster> TaskStatusRepo { get; }

        public MasterRepository<TaskPriorityMaster> TaskPriorityRepo { get; }

        public MasterRepository<TaskUserMapping> TaskUserMappingRepo { get; }

        public DashboardRepository DashboardRepo { get; }

        public MasterRepository<TaskTrail> TaskTrailRepo { get; }
        public MasterRepository<CategoryMaster> CategoryRepo { get; }
        public AttachmentRepository AttachmentRepo { get; }

        public TaskUserRecordRepository TaskUserRecordRepo { get; }

        public MasterRepository<EmailTemplate> EmailTemplateRepo { get; }

        public EmployeeRepository EmployeeRepo { get; }

        //public MasterRepository<ProjModUserMapping> ProjModUserMappingRepo { get; }
        public ProjModUserMapRepository ProjModUserMappingRepo { get; }
        public GroupRepository GroupRepo { get; }
        public MasterRepository<GroupMember> GroupMemberRepo { get; }
        public AccessControlRepository AccessControlRepo { get; }

        public MasterRepository<ModuleLevelDetail> ModuleLevelDetailRepo { get; }
        public MasterRepository<ModuleLevelUserMapping> ModuleLevelUserMappingRepo { get; }

        public MasterRepository<EnvironmentMaster> EnvironmentRepo { get; }

        public MasterRepository<ProjectParameterMaster> ProjParamRepo { get; }


        public MasterRepository<ProjectDetails> ProjDetailsRepo { get; }


        public ReleaseRepository ReleaseInstRepo { get; }

        public MasterRepository<ReleaseDetails> ReleaseDetailsRepo { get; }


        #endregion

        #region Common Email Repository Declaration
        public EmailRepository EmailRepo { get; }
        #endregion
        public UnitOfWork()
        {
            _TTContext = new TTDBContext();
            _CEContext = new CEDBContext();

            ///ToDo: Below region code should be removed before moving to live, for improving the performance of the application.
            #region For Debugging purpose to see the generated SQL from LINQ.
            _TTContext.Database.Log += tt => Debug.WriteLine(tt);
            _CEContext.Database.Log += ce => Debug.WriteLine(ce);
            #endregion

            #region Task tracker Repository Initialization
            CommonRepo = new CommonRepository(_TTContext);
            LoginRepo = new LoginRepository();
            ProgressRepo = new MasterRepository<ProgressMaster>(_TTContext);
            ProjectsRepo = new ProjectRepository(_TTContext);
            TasksRepo = new TaskRepository(_TTContext);
            ModuleWFMapRepo = new ModuleWFMapRepository(_TTContext);
            ModuleRepo = new ModuleRepository(_TTContext);
            WorkflowRepo = new MasterRepository<WorkflowMaster>(_TTContext);
            WorkflowLevelRepo = new MasterRepository<WorkflowLevelDetails>(_TTContext);
            TaskStatusRepo = new MasterRepository<TaskStatusMaster>(_TTContext);
            TaskPriorityRepo = new MasterRepository<TaskPriorityMaster>(_TTContext);
            TaskUserMappingRepo = new MasterRepository<TaskUserMapping>(_TTContext);
            TaskTrailRepo = new MasterRepository<TaskTrail>(_TTContext);
            CategoryRepo = new MasterRepository<CategoryMaster>(_TTContext);
            AttachmentRepo = new AttachmentRepository(_TTContext);
            TaskUserRecordRepo = new TaskUserRecordRepository(_TTContext);
            DashboardRepo = new DashboardRepository(_TTContext);
            EmailTemplateRepo = new MasterRepository<EmailTemplate>(_TTContext);
            EmployeeRepo = new EmployeeRepository(_TTContext);
            ProjModUserMappingRepo = new ProjModUserMapRepository(_TTContext);
            GroupRepo = new GroupRepository(_TTContext);
            GroupMemberRepo = new MasterRepository<GroupMember>(_TTContext);
            AccessControlRepo = new AccessControlRepository(_TTContext);
            ModuleLevelDetailRepo = new MasterRepository<ModuleLevelDetail>(_TTContext);
            ModuleLevelUserMappingRepo = new MasterRepository<ModuleLevelUserMapping>(_TTContext);
            EnvironmentRepo = new MasterRepository<EnvironmentMaster>(_TTContext);
            ProjParamRepo = new MasterRepository<ProjectParameterMaster>(_TTContext);
            ProjDetailsRepo = new MasterRepository<ProjectDetails>(_TTContext);
            ReleaseInstRepo = new ReleaseRepository(_TTContext);
            ReleaseDetailsRepo = new MasterRepository<ReleaseDetails>(_TTContext);
            #endregion

            #region Common Email Repository initialization
            EmailRepo = new EmailRepository(_CEContext, _TTContext);
            #endregion
        }
        
        /// <summary>
        /// This method is used to commit the changes after all the save operations have been completed.        
        /// </summary>
        /// <returns>Returns integer count of the number of records changed or saved in the database.</returns>
        public int commitTT()
        {
            return _TTContext.SaveChanges();
        }
        public int commitCE()
        {
            return _CEContext.SaveChanges();
        }
        public int commitAll()
        {
            int ttCommit = _TTContext.SaveChanges();
            int ceCommit = _CEContext.SaveChanges();
            return ttCommit & ceCommit;
        }

        #region Dispose implementation
        /// <summary>
        /// This boolean property is used to decide whether the object of the class is already disposed or not.
        /// Else, when it is false and not already disposed and Dispose is called for the object when scoped out then it is set to true while disposing.
        /// This is to ensure that we are not disposing the DBContext instance which is already disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Overloaded version of below function to check if the object is not already disposed and garbage collected by CLR.
        /// This is called internally by below funtion or the inheriting class.
        /// </summary>
        /// <param name="disposing">Flag to differentiat the overloading of method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _TTContext.Dispose();
                    _CEContext.Dispose();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Called when the object of UnitOfWork goes out of scope like when the using block ends or when the class in which object of UnitOfWork is instanciated, goes out of scope,
        /// and disposed of.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
