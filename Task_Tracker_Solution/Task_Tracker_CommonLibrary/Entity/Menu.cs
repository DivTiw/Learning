using System.ComponentModel.DataAnnotations;

namespace Task_Tracker_CommonLibrary.Entity
{
    [System.ComponentModel.DataAnnotations.Schema.Table("task_menu_master")]
    public class Menu
    {
        [Key]
        public int menu_syscode { get; set; }

        public string menu_name { get; set; }

        public string menu_description { get; set; }

        public int? parent_menu_syscode { get; set; }

        public string page_url { get; set; }

        public bool is_enabled { get; set; }

        public int display_order { get; set; }

        public string icon { get; set; }
    }

}
