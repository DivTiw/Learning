using System.Collections.Generic;
using Task_Tracker_CommonLibrary.Others;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    public class DDLDTO : OperationDetailsDTO
    {
        ///ToDo: Enhance DDLDTO to handle condition based drop down fetching.
        //public bool shouldGetData { get; set; }
        //public DBTableNameEnums[] EntityNames { get; set; }
        public List<DBTableNameEnums> EntityNames { get; set; }
        public IDictionary<DBTableNameEnums, Dictionary<string, object>> Predicate { get; set; }
        public IDictionary<DBTableNameEnums, List<SelectItemDTO>> Data { get; set; }
        public IDictionary<DBTableNameEnums, List<int>> DisabledValues { get; set; }
        public DDLDTO()
        {
            //shouldGetData = false;
            Data = new Dictionary<DBTableNameEnums, List<SelectItemDTO>>();
        }
        public DDLDTO(List<DBTableNameEnums> _lstDDLEntityNames, bool _shouldGetData = false)
        {
            //EntityNames = _lstDDLEntityNames == null ? null : _lstDDLEntityNames.ToArray();
            EntityNames = _lstDDLEntityNames == null ? new List<DBTableNameEnums>() : _lstDDLEntityNames;
            //shouldGetData = _shouldGetData;
            Predicate = new Dictionary<DBTableNameEnums, Dictionary<string, object>>();
            DisabledValues = new Dictionary<DBTableNameEnums, List<int>>();
            foreach (var entity in EntityNames) 
            {
                Predicate.Add(entity,
                                new Dictionary<string, object>()
                                {
                                    { "GetData",false },
                                    { "is_deleted", false },
                                    { "is_active", true }
                                }
                              );
                DisabledValues.Add(entity, new List<int>() { });
            }
            Data = new Dictionary<DBTableNameEnums, List<SelectItemDTO>>();
        }
    }
}
