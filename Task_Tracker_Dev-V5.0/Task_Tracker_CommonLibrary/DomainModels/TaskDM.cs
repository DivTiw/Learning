using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    [NotMapped]
    public class TaskDM : JMTask
    {
        public TaskDM()
        {
            project = new ProjectMaster();
            module = new ModuleMaster();
            status = new TaskStatusMaster();
            category = new CategoryMaster();
            lstAttachments = new List<TaskAttachment>();
            lstTrail = new List<TaskTrail>();            
            lstTrailCommentsVM = new List<TrailFileCommentVM>();
            taskUserRecord = new TaskUserRecord();
            lstSubtasks = new List<JMTask>();
            priority = new TaskPriorityMaster();
            workflowLevels = new ModuleWFLevelMapDM();
            PageFieldAccesses = new Dictionary<string, bool>();
            //lstRelDetails = new List<ReleaseDetails>();
        }
                
        public TrailFileCommentVM TrailComVM { get; set; }        
        public int? group_syscode { get; set; }
        public ProjectMaster project { get; set; }
        public ModuleMaster module { get; set; }
        public TaskStatusMaster status { get; set; }
        public CategoryMaster category { get; set; }
        public List<TaskAttachment> lstAttachments { get; set; }
        public string workflow_name { get; set; }
        public string level_name { get; set; }
        public string members { get; set; }
        public int[] arrUserSyscodes { get; set; }
        public int[] arrCCUsersSyscodes { get; set; }
        public string task_comment { get; set; }
        public List<LoginUser> lstUsers { get; set; }
        public List<TaskTrail> lstTrail { get; set; }
     
        public List<TrailFileCommentVM> lstTrailCommentsVM { get; set; }

        [DisplayName("Owner")]
        public string owner { get; set; }

        public string onBehalf { get; set; }

      
        public string strCreatedBy { get; set; }

        [DisplayName("Progress")]
        public decimal? progress { get; set; }

        public DDLDTO ddlData = new DDLDTO(new List<DBTableNameEnums>() { DBTableNameEnums.TaskPriorityMaster,
                                                                          DBTableNameEnums.TaskStatusMaster,
                                                                          DBTableNameEnums.vw_employee_master,
                                                                          DBTableNameEnums.CategoryMaster,
                                                                          DBTableNameEnums.GroupMember});
        public bool isWorkflowTask { get; set; }
        public bool isSubTask { get; set; }
        public bool isTaskStarted { get; set; }
        public string parentTaskRef { get; set; }

        public TaskUserRecord taskUserRecord { get; set; }
        public string startedTaskRefNo { get; set; }

        public string strActionedBy { get; set; }

        public List<JMTask> lstSubtasks { get; set; }

        public TaskPriorityMaster priority { get; set; }

        public List<TaskTreeDM> lstTaskTrees { get; set; }

        //[DisplayName("Modified By")]
        //public string modified_by_name { get; set; }
        public string CurrentStartedTaskRefNo { get; set; }

        public ModuleWFLevelMapDM workflowLevels { get; set; }

        public  bool isToDoTask { get; set; }
        public bool blnCreateLevelTasks { get; set; } = true;

        public Enum_Master.TaskOperationState Operation;

        public Dictionary<string, bool> PageFieldAccesses { get; set; }

       // public List<ReleaseDetails> lstRelDetails { get; set; }
        public string releaseDetailsJson { get; set; }

        public string parentTaskSubject { get; set; }
    }
}
