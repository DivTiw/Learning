using System.Runtime.Serialization;

namespace Task_Tracker_CommonLibrary.Others
{
    //[JsonConverter(typeof(StringEnumConverter))]
    public enum DBTableNameEnums
    {
        [EnumMember(Value = "Tasks")]
        Tasks,

        [EnumMember(Value = "AccessGrpMaster")]
        AccessGrpMaster,

        [EnumMember(Value = "AccessGrpMnuDtl")]
        AccessGrpMnuDtl,

        [EnumMember(Value = "TaskActivityMaster")]
        TaskActivityMaster,

        [EnumMember(Value = "TaskAttachment")]
        TaskAttachment,

        [EnumMember(Value = "TaskCategoryContact")]
        TaskCategoryContact,

        [EnumMember(Value = "CategoryMaster")]
        CategoryMaster,

        [EnumMember(Value = "TaskLoginHistory")]
        TaskLoginHistory,

        [EnumMember(Value = "TaskPriorityMaster")]
        TaskPriorityMaster,

        [EnumMember(Value = "TaskStatusMaster")]
        TaskStatusMaster,

        [EnumMember(Value = "TaskTrail")]
        TaskTrail,

        [EnumMember(Value = "TaskTrailDetail")]
        TaskTrailDetail,

        [EnumMember(Value = "TaskUserMapping")]
        TaskUserMapping,

        [EnumMember(Value = "TaskUserRoleMaster")]
        TaskUserRoleMaster,

        [EnumMember(Value = "ProjectMaster")]
        ProjectMaster,

        [EnumMember(Value = "ModuleMaster")]
        ModuleMaster,

        [EnumMember(Value = "WorkflowMaster")]
        WorkflowMaster,

        [EnumMember(Value = "WorkflowLevelDetails")]
        WorkflowLevelDetails,

        [EnumMember(Value = "vw_employee_master")]
        vw_employee_master,

        [EnumMember(Value = "vw_department_master")]
        vw_department_master,

        [EnumMember(Value = "GroupMaster")]
        GroupMaster,

        [EnumMember(Value = "GroupMasterByEmp")]
        GroupMasterByEmp,

        [EnumMember(Value = "GroupMember")]
        GroupMember,

        [EnumMember(Value = "EnvironmentMaster")]
        EnvironmentMaster,

        [EnumMember(Value = "ProjectParameterMaster")]
        ProjectParameterMaster
    }
}
