using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("group_members")]
    public class GroupMember : JMBaseBusinessEntity
    {
        [Key]
        public int group_member_syscode { get; set; }

        public int group_syscode { get; set; }
        public int employee_syscode { get; set; }
        public int role_syscode { get; set; }
        public GroupMaster groupMaster { get; set; }
    }
}
