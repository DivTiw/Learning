using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Tracker_CommonLibrary.Entity
{
    [Table("task_attachment")]
    public class TaskAttachment : JMBaseBusinessEntity
    {
        [Key]
        public int attachment_syscode { get; set; }
        
        public Guid attachment_identifier { get; set; }
        
        public string mongo_file_id { get; set; }
        [MaxLength(200)]
        public string attachment_filename { get; set; }
        [MaxLength(200)]
        public string attachment_display_name { get; set; }
        [MaxLength(200)]
        public string type_detail { get; set; } //Eg. Task or Trail
        public int type_syscode { get; set; } //Eg. if Type is Task then it will come from TaskMaster else in case of Trail, it will come from the TrailMaster.
        [NotMapped]
        public override bool is_active{ get; set; } // Currently not mapped as the column does not exists in the database table.
    }
}
