using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("group_master")]
    public class GroupMaster : JMBaseBusinessEntity
    {
        [Key]
        public int group_syscode { get; set; }

        [DisplayName("Group")]
        public string group_name { get; set; }

        [DisplayName("Description")]
        public string group_description { get; set; }

        public string group_email_id { get; set; }

        public IList<GroupMember> lstGroupMembers { get; set; }
    }
}
