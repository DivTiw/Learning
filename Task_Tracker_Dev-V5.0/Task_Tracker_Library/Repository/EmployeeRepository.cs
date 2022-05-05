using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.Entity;

namespace Task_Tracker_Library.Repository
{
    public class EmployeeRepository : TTBaseRepository<TT_vw_employee_master>
    {
        public EmployeeRepository(TTDBContext context) : base(context)
        {            
        }

        public List<string> GetNamesBySyscode(List<int> empSyscode)
        {
            var empNames = (from emps in context.vw_employee_master
                            where empSyscode.Contains(emps.employee_syscode ?? 0)
                            select emps.employee_name
                            ).ToList();

            return empNames;
        }

        public Dictionary<int, string> GetEmpNamesBySyscode(List<int> empSyscode)
        {
            var empNames = (from emps in context.vw_employee_master
                            where empSyscode.Contains(emps.employee_syscode ?? 0)
                            select new  {emps.employee_syscode, emps.employee_name }
                            ).ToDictionary(x => x.employee_syscode ?? 0, y => y.employee_name);

            return empNames;
        }
    }
}
