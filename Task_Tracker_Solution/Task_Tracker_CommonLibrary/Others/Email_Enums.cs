using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Others
{
    public class Email_Enums
    {
        public enum Email_Type
        {
            [Description("Workflow Creation")]
            WFC,
            [Description("Workflow Updation")]
            WFU,
            [Description("Project Creation")]
            PC,
            [Description("Project Updation")]
            PU,
            [Description("Module Creation")]
            MC,
            [Description("Module Updation")]
            MU,
            [Description("Task Creation")]
            TC,
            [Description("Task Updation")]
            TU,
            [Description("Task Status Change")]
            TSC,
            [Description("Task Weightage Change")]
            TWC,
            [Description("Task Member Update")]
            TMU,
            [Description("Task Inform To")]
            TINFT,
            [Description("Task Creation Workflow Task Parent")]
            TCWFP
        }

        public enum Email_Recipients
        {
            [Description("NA")]
            NA,
            [Description("Task Owner")]
            TWNR,
            [Description("Task Members")]
            TMEM,
            [Description("Task Creator")]
            TCRTR,
            [Description("Module All")]
            MAll,
            [Description("Project All")]
            PAll,
            [Description("Group All")]
            GAll,
            [Description("Task On-Behalf")]
            TONBF,
            [Description("Module Creator")]
            MCRTR,
            [Description("Module Write")]
            MWrite,
            [Description("Module Read")]
            MRead,
            [Description("Project Write")]
            PWrite,
            [Description("Project Read")]
            PRead,
            [Description("Project Creator")]
            PCRTR,
            [Description("Group Creator")]
            GCRTR,
            [Description("Group Heads")]
            GHeads,
            [Description("Group Read")]
            Gread,
            [Description("Task Inform-To")]
            TINFT,
            [Description("Project Module All")]
            PMAll,
            [Description("Project Module Write Users")]
            PMWrite,
            [Description("Project Module Read Users")]
            PMRead
        }
    }
}
