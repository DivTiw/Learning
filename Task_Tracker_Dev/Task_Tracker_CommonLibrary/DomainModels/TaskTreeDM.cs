using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    public class TaskTreeDM
    {
        public TaskTreeDM()
        {
            state = new NodeState();
        }
        public int task_syscode { get; set; }
        public string task_ref { get; set; }
        public int? parent_syscode { get; set; }       
        public string text { get; set; }
        public string href { get; set; }
        public NodeState state { get; set; }
        public List<TaskTreeDM> nodes { get; set; }
    }

    public class NodeState
    {
        public bool expanded { get; set; }
        public bool selected { get; set; }
    }
}
