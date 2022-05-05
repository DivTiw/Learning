using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    [NotMapped]
    public class TaskUserRecordDM :TaskUserRecord
    {
        public string UserAction { get; set; }

    }
}
