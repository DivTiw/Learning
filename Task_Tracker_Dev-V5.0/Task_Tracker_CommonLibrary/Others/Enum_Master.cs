using System.ComponentModel;
using System.Runtime.Serialization;

namespace Task_Tracker_CommonLibrary.Others
{
    public class Enum_Master
    {
        public enum ProgressTypeEnum
        {
            [EnumMember(Value = "Project")]
            Project,

            [EnumMember(Value = "Module")]
            Module,

            [EnumMember(Value = "Task")]
            Task,

            //[EnumMember(Value = "Trail")]
            //Trail
        }

        public enum UserRoleEnum
        {
            Creator = 1,
            Owner = 2,
            Member = 3,
            Created_For = 4,
            Project_User = 5,
            Module_User = 6,
            Group_Head = 7,
            Group_Creator = 8
        }

        public enum PriorityEnum
        {
            High = 1,
            Medium = 2,
            Low = 3
        }

        public enum StatusEnum
        {
            [Description("Open")]
            Open = 1,

            [Description("In Progress")]
            InProgress = 3,

            [Description("Complete")]
            Complete = 5,

            [Description("Initiate")]
            Initiate = 6,

            [Description("Acknowledge")]
            Acknowledge = 7,

            [Description("On Hold")]
            OnHold = 8,

            [Description("Discard")]
            Discard = 9,

            [Description("ToDo")]
            ToDo = 10

            //[Description("Invalid")]
            //Invalid = 2,            
            //[Description("Inactive")]
            //Inactive = 4,                     
        }

        public enum ActivityEnum
        {
            [Description("Created")]
            Created = 1,

            [Description("Acknowledged")]
            Acknowledged = 2,

            [Description("Started")]
            Started = 3,

            [Description("Changed Weightage")]
            Changed_Weightage = 4,

            [Description("Created for")]
            Created_For = 5,

            [Description("Forwarded")]
            Forwarded = 6,

            [Description("Completed")]
            Completed = 7,

            [Description("Closed")]
            Closed = 8,

            [Description("Changed Status")]
            Changed_Status = 9,

            [Description("Added Member(s)")]
            Added_Member = 10,

            [Description("Added File(s)")]
            Added_File = 11,

            [Description("Added Comments")]
            Added_Comments = 12,

            [Description("Created Subtask")]
            Created_Subtask = 13,

            [Description("Removed Member(s)")]
            Removed_Member = 14,

            [Description("Informed")]
            Informed_To = 15
        }
        public enum TaskType
        {
            [EnumMember(Value = "SA")]
            StandAlone,
            [EnumMember(Value = "WF")]
            Workflow
        }

        public enum TaskOperationState
        {
            Creation,
            Updation
        }


    }
    public enum ReportsEnum
    {
        [Description("Task Activity Mgmt Report")]
        TAMR = 1
    }
}
