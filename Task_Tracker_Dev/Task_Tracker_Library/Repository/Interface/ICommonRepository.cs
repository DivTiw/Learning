using System.Collections.Generic;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Library.Interface
{
    //Module master as a type is just used for facilitating inheritance, none of the methods from base would be utilised that are directly casted to the ModuleMaster type.
    //Need to look for the better approach.
    public interface ICommonRepository : ITTBaseRepository<ModuleMaster>
    {        
        DDLDTO fillDDLdata(DDLDTO ddlObj);
        string getEmployeeNames(IEnumerable<int> arrUserSyscodes);
    }
}
