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
    public class StandAloneTask : AbstractTask
    {
        public override void Attach(TaskDM task, UnitOfWork uow = null)
        {
            _task_type = Enum_Master.TaskType.StandAlone;
            base.Attach(task, uow); 
        }
        public override OperationDetailsDTO Process()
        {
            base.Process();
            OperationDetailsDTO od = new OperationDetailsDTO();
            od.opStatus = true;
            return od;
        }

       ///ToDO Override BuildEmail code here for standalone tasks
    }
}
