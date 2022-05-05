using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Task_Tracker_CommonLibrary.DomainModels;

namespace Task_Tracker_CommonLibrary.Entity
{
    public abstract class JMBaseBusinessEntity : OperationDetailsDTO
    {
        [NotMapped]
        public virtual string created_by_Name { get; set; }
        public virtual int created_by { get; set; }


        [DisplayFormat(DataFormatString = "{0:ddd, dd-MMM-yyyy HH:mm}")]
        public virtual DateTime created_on { get; set; } = DateTime.Now;

        [NotMapped]
        [DisplayName("Modified By")]
        public virtual string modified_by_Name { get; set; }
        public virtual int? modified_by { get; set; }

        [DisplayFormat(DataFormatString = "{0:ddd, dd-MMM-yyyy HH:mm}")]
        public virtual DateTime? modified_on { get; set; }
        public virtual bool is_active { get; set; } = true;
        public virtual bool is_deleted { get; set; }        
    }
}
