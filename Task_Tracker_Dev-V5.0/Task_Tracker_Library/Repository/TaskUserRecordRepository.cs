using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Library.Repository
{
    public class TaskUserRecordRepository : TTBaseRepository<TaskUserRecord>
    {
        ///ToDo: Prority: LOW: Check to see if you need this class anymore as it doesn't have any methods or fields.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public TaskUserRecordRepository(TTDBContext context) : base(context)
        {
        }

        //public TaskUserRecord getMyOpenTaskByID(int task_syscode, int emp_syscode)
        //{
        //    TaskUserRecord DM = new TaskUserRecord();

        //    DM = (from tsk in context.TaskUserRecord
        //          where tsk.task_syscode.Equals(task_syscode) && tsk.employee_syscode.Equals(emp_syscode) && tsk.stop_time == null
        //          select new TaskUserRecordDM
        //          {
        //              record_syscode = tsk.record_syscode,
        //              task_syscode = tsk.task_syscode,
        //              start_time = tsk.start_time,
        //              employee_syscode = tsk.employee_syscode
        //              ,
        //          }).FirstOrDefault();
        //    return DM;
        //}
    }
}
