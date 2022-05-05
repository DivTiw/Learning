using System.ComponentModel.DataAnnotations.Schema;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    [NotMapped]
    public class EmailTemplateDM : EmailTemplate
    {
        [NotMapped]
        public int Email_Def_Syscode { get; set; }
        [NotMapped]
        public string str_To_CatCodes { get; set; }
        [NotMapped]
        public string str_Cc_CatCodes { get; set; }
    }
}
