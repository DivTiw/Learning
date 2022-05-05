using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_Library.Factories.TaskFactory
{
    public static class FactoryMethod
    {
        public static AbstractTask GetTaskInstance(Enum_Master.TaskType _EnumTaskType)
        {
            AbstractTask at = null;
            switch (_EnumTaskType)
            {
                case Enum_Master.TaskType.StandAlone:
                    at = new StandAloneTask();
                    break;
                case Enum_Master.TaskType.Workflow:
                    at = new WorkflowTask();
                    break;
                default:
                    at = null;
                    break;
            }

            return at;
        }
    }
}
