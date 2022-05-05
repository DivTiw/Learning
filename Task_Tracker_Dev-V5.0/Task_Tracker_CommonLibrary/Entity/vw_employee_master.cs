using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Task_Tracker_CommonLibrary.Entity
{    
   
    public class vw_employee_master
    {
        public int? company_syscode { get; set; }
        public string company_name { get; set; }
        public string location_name { get; set; }
        public string department_name { get; set; }
        public string designation_name { get; set; }
        [Key]
        public int? employee_syscode { get; set; }
        public int? employee_code { get; set; }
        public string employee_name { get; set; }
        public string employee_first_name { get; set; }
        public string employee_middle_name { get; set; }
        public string employee_last_name { get; set; }
        public string gender { get; set; }
        public DateTime? dob { get; set; }
        public DateTime? doj { get; set; }
        public DateTime? doc { get; set; }
        public string confirmation_status { get; set; }
        public string employee_function_type { get; set; }
        public int? designation_syscode { get; set; }
        public int? grade_syscode { get; set; }
        public int? department_syscode { get; set; }
        public int? function_sector_covered_syscode { get; set; }
        public int? reporting_manager_syscode { get; set; }
        public int? functional_manager_syscode { get; set; }
        public int? skip_level_reporting_manager { get; set; }
        public string relevant_prior_experience { get; set; }
        public string non_relevant_prior_experience { get; set; }
        public string total_years { get; set; }
        public char re_employed { get; set; }
        public string email_id { get; set; }
        public string desk_phone { get; set; }
        public string extension { get; set; }
        public string home_no { get; set; }
        public string mobile_no { get; set; }
        public string permanent_address { get; set; }
        public string temporary_address { get; set; }
        public string blood_group { get; set; }
        public string pan_no { get; set; }
        public string bank_account_no { get; set; }
        public string pf_no { get; set; }
        public int? location_syscode { get; set; }
        public string employment_status { get; set; }
        public string marital_status { get; set; }
        public string nationality { get; set; }
        public string fathers_name { get; set; }
        public string emergency_contact_name { get; set; }
        public string emergency_contact_address { get; set; }
        public string emergency_phone_number { get; set; }
        public string relationship { get; set; }
        public string passport_number { get; set; }
        public DateTime? passport_issue_date { get; set; }
        public string passport_issue_place { get; set; }
        public DateTime? passport_expiry_date { get; set; }
        public char hris_filled { get; set; }
        public decimal? total_leaves { get; set; }
        public int? last_company_designation_syscode { get; set; }
        public string windows_login_id { get; set; }
        public char is_active { get; set; }
        public string my_brokerage_id { get; set; }
        public string password { get; set; }
        public decimal? total_current_leaves { get; set; }
        public int? ed_syscode { get; set; }
        public int? coed_syscode { get; set; }
        public int? state_syscode { get; set; }
        public int? sub_location_syscode { get; set; }
        public int? departmental_cordinator_syscode { get; set; }
        public DateTime? resigned_on { get; set; }
        public string resigned_reason { get; set; }
        public DateTime? last_working_date { get; set; }
        public string resigned_remarks { get; set; }
        public string special_resigned_remarks { get; set; }
        public int? function_type_syscode { get; set; }
        public virtual int? employment_type_syscode { get; set; }
        public virtual int? insurance_type_syscode { get; set; }
        public virtual int? confirmation_ed_syscode { get; set; }
        public virtual int? confirmation_coed_syscode { get; set; }
        public string calling_name { get; set; }
        public virtual bool? is_autoconfirmed { get; set; }
        public virtual string sub_department_name { get; set; }

    }
}
