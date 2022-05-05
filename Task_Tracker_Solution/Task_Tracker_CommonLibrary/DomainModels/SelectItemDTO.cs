using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    [NotMapped]
    public class SelectItemDTO //: OperationDetailsDTO
    {
        public string Text { get; set; }
        public object Value { get; set; }
        public object SelectedItem { get; set; }
    }

    //public class PredicateDTO
    //{
    //    public string Key { get; set; }
    //    public object Value { get; set; }
    //    public PredicateDTO(string key, object value)
    //    {
    //        Key = key;
    //        Value = value;
    //    }
    //}
}
