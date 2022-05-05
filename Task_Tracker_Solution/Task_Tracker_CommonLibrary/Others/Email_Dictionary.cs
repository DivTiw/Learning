using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Others
{
    public class Email_Dictionary
    {
        public enum EmailConditions
        {
            To,
            CC,
            Subject
        }
        public enum ToEmailRecipients
        {
            NA,
            All,
            Members,
            Creator
        }
        public enum CCEmailRecipients
        {
            NA,
            All,
            Members,
            Creator
        }

        public static Dictionary<Enum_Master.StatusEnum, string> EmailTemplateName = new Dictionary<Enum_Master.StatusEnum, string>
        {
            { Enum_Master.StatusEnum.Initiate, "Task_Initiated" },
            { Enum_Master.StatusEnum.Acknowledge, "Task_Acknowledged" },
            { Enum_Master.StatusEnum.InProgress, "Task_Inprogress" },
            { Enum_Master.StatusEnum.OnHold, "Task_OnHold" },
            { Enum_Master.StatusEnum.Discard, "Task_Discarded" },
            { Enum_Master.StatusEnum.Complete, "Task_Completed" }
        };
    }
}
