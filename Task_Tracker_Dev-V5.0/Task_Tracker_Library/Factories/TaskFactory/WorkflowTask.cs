using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Library.Repository;

namespace Task_Tracker_Library.Factories.TaskFactory
{
    public class WorkflowTask : AbstractTask
    {
        public WorkflowTask()
        {
        }

        public override void Attach(TaskDM task, UnitOfWork uow = null)
        {
            _task_type = Enum_Master.TaskType.Workflow;
            base.Attach(task, uow);
        }
        
        public override OperationDetailsDTO Process()
        {
            OperationDetailsDTO od = new OperationDetailsDTO();
            
            base.Process();

            od.opStatus = true;
            od.opMsg = "Task successfully processed.";
            return od;
        }

        public override OperationDetailsDTO Complete()
        {
            return base.Complete();
        }
    }
}
