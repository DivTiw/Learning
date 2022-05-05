using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrideWindowsSchedular
{
    public class Program
    {
        public static void Main(string[] args)
        {
            TaskActivityMGMTRpt rpt = new TaskActivityMGMTRpt();
            var result = rpt.Run();
            //Console.ReadKey();
        }        

    }
}
