using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_Tracker_CommonLibrary.DomainModels
{
    public class LmsAttndDTO
	{
		[DisplayName("Emp Code")]
        public string Emp_Code { get; set; }

		[DisplayName("Emp Name")]
		public string Employee_Name { get; set; }
		public string Day { get; set; }
		public string Date { get; set; }

		[DisplayName("Actual In Time")]
		public string Actual_In_Time { get; set; }

		[DisplayName("Actual Out Time")]
		public string Actual_Out_Time { get; set; }

		[DisplayName("Regularized In Time")]
		public string Regularized_In_Time { get; set; }

		[DisplayName("Regularized Out Time")]
		public string Regularized_Out_Time { get; set; }
		public int Hours { get; set; }
		public string Description { get; set; }

		[DisplayName("Leave Type")]
		public string Leave_type { get; set; }

		[DisplayName("Attendance Source")]
		public string Attendance_Source { get; set; }
       
	}
}
