using System.ComponentModel.DataAnnotations;
namespace Task_Tracker_CommonLibrary.Entity
{
    [System.ComponentModel.DataAnnotations.Schema.Table("category_master")]
    public class CategoryMaster : JMBaseBusinessEntity
    {
        [Key]
        public int category_syscode { get; set; }

        [MaxLength(200)]
        public string category_name { get; set; }

        public int? group_syscode { get; set; }

        //public int department_syscode { get; set; }
        //public string email_id { get; set; }
        //public vw_department_master vw_Dept { get; set; }

    }
}
