using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SpreadsheetGear;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Library.Repository;

namespace StrideWindowsSchedular
{
    public class TaskActivityMGMTRpt
    {
        public ReportsEnum report { get; private set; }

        public TaskActivityMGMTRpt()
        {
            report = ReportsEnum.TAMR;
        }
        public bool Run()
        {
            try
            {
                IList<GroupMember_TaskDtlsDTO> lstMemTaskDtls = null;
                lstMemTaskDtls = GetMemberTaskDtls();

                IEnumerable<int> lstDstinctEmps = lstMemTaskDtls.Select(x => x.employee_syscode).Distinct();

                if (lstDstinctEmps == null || lstDstinctEmps.Count() == 0)
                    throw new Exception("Task Data list is empty");

                int rptSpan = 7; //Days
                DateTime _today = DateTime.Now.Date;
                DateTime rptStartDate = _today.AddDays(-rptSpan);
                DateTime rptEndDate = _today.AddDays(-1);
                string rptStartDateString = rptStartDate.ToString("dddd, dd MMM yyyy");
                string rptEndDateString = rptEndDate.ToString("dddd, dd MMM yyyy");

                IWorkbook eWorkbook = SpreadsheetGear.Factory.GetWorkbook();
                string filename = "StrideActivityReport_" + DateTime.Now.ToString("d");

                using (var uow = new UnitOfWork())
                {
                    GroupReportDM grpRptDm = uow.SchReportsRepo.getReportDetails(report, 1);//Hardcoding for group 1 for now.

                    if (grpRptDm == null)
                    {
                        throw new Exception("Group Report details not found.");
                    }

                    EmailTemplate temp = uow.EmailTemplateRepo.GetList(x => x.template_syscode.Equals(grpRptDm.template_syscode) && x.is_active).FirstOrDefault();

                    if (temp == null)
                    {
                        throw new Exception("Email Template Not found.");
                    }

                    foreach (var emp in lstDstinctEmps)
                    {
                        IList<GroupMember_TaskDtlsDTO> lstEmpTD = lstMemTaskDtls.Where(x => x.employee_syscode == emp).ToList();
                        string emp_Name = lstEmpTD[0].employee_name;
                        //rptStartDate = rptStartDate.AddYears(-1);
                        DataTable dt = uow.SchReportsRepo.proc_LMS_GetDailyAttendanceReport(emp, rptStartDate, rptEndDate);

                        createEmpSheet(eWorkbook, emp_Name, lstEmpTD, dt);
                    }

                    eWorkbook.Worksheets["Sheet1"].Delete();
                    eWorkbook.Worksheets[0].Select();

                    byte[] fileBytes = eWorkbook.SaveToMemory(FileFormat.Excel8);
                    string strFileBase64 = Convert.ToBase64String(fileBytes);

                    XElement EmailAttach = new XElement("EmailAttach",
                                           new XAttribute("attachment_content", strFileBase64)
                                         , new XAttribute("original_file_name", filename + ".xls")
                                         , new XAttribute("file_name", filename + ".xls"));

                    string fileXML = EmailAttach.ToString();

                    //eWorkbook.SaveAs(filename + ".xls", FileFormat.Excel8);
                    Console.WriteLine("Excel created successfully.");
                    SendMail(rptStartDateString, rptEndDateString, uow, temp, fileXML, grpRptDm);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

        }

        private void createEmpSheet(IWorkbook eWorkbook, string emp_Name, IEnumerable<GroupMember_TaskDtlsDTO> lstEmpTD, DataTable dtEmpAttnd)
        {
            decimal totalHours = 0.00m;
            decimal totalDays = 0.00m;

            totalHours = lstEmpTD.Sum(x => x.Total_Hours_Worked ?? 0.00m);
            totalDays = lstEmpTD.Sum(x => x.Total_Days_Worked ?? 0.00m);

            DataTable dtEmpTD = ComLibCommon.IEnumerableToDataTable(lstEmpTD);
            dtEmpTD.Columns.Add(new DataColumn { ColumnName = "SR No", Caption = "SR No", DataType = typeof(int) });

            IWorksheet eWorksheet = eWorkbook.Worksheets.Add();
            eWorksheet.Name = emp_Name;

            //Task report            
            int tdColCount = dtEmpTD.Columns.Count;
            int tdRowCount = dtEmpTD.Rows.Count;
            string strSerialColName = "SR No";
            dtEmpTD.RemoveColsSetOrder(new string[] { strSerialColName
                                                  , nameof(GroupMember_TaskDtlsDTO.project_name)
                                                  , nameof(GroupMember_TaskDtlsDTO.module_name)
                                                  , nameof(GroupMember_TaskDtlsDTO.Parent_Subject)
                                                  , nameof(GroupMember_TaskDtlsDTO.task_subject)
                                                  , nameof(GroupMember_TaskDtlsDTO.status_name)
                                                  , nameof(GroupMember_TaskDtlsDTO.Total_Hours_Worked)
                                                  , nameof(GroupMember_TaskDtlsDTO.Total_Days_Worked)
                                                 });
            for (int i = 0; i < tdRowCount; i++)
            {
                dtEmpTD.Rows[i][strSerialColName] = i + 1;
            }

            IRange eCells = eWorksheet.Cells;
            eCells.Columns.ColumnWidth = 20;

            IRange header = eCells["A1:H1"];
            header.Merge();
            header.Value = "Task Activity Report";
            header.EntireRow.Font.Bold = true;

            int tblStartRow = 3;
            IRange TDCellRange = eCells["A" + tblStartRow];
            TDCellRange.EntireRow.Font.Bold = true;
            //if (tdRowCount > 0)
            //{
            TDCellRange.CopyFromDataTable(dtEmpTD, SpreadsheetGear.Data.SetDataFlags.None);
            //}
            //else
            int cntUsedRange = eWorksheet.UsedRange.RowCount;
            IRange rngNoRecords;

            if (lstEmpTD.Count() == 1 && lstEmpTD.First().task_syscode == null)
            {
                rngNoRecords = eCells[$"A{cntUsedRange}:H{cntUsedRange}"];
                rngNoRecords.EntireRow.Clear();
                rngNoRecords.Merge();
                rngNoRecords.Value = "No Records Found.";
                rngNoRecords.EntireRow.Font.Bold = true;
            }
            else
            {
                cntUsedRange = cntUsedRange + 1;
                eCells["F" + cntUsedRange].EntireRow.Font.Bold = true;
                eCells["F" + cntUsedRange].Value = "Total";
                eCells["G" + cntUsedRange].Value = totalHours;
                eCells["H" + cntUsedRange].Value = totalDays;
            }


            IRange IR1 = eCells[$"A{tblStartRow}:H{cntUsedRange}"];
            SetBorders(IR1);
            //Attendance Report
            cntUsedRange = cntUsedRange + 2;
            IRange AttndCellheader = eCells[$"A{cntUsedRange}:H{cntUsedRange}"];
            AttndCellheader.Merge();
            AttndCellheader.Value = "Attendance Report";
            AttndCellheader.EntireRow.Font.Bold = true;

            cntUsedRange = cntUsedRange + 2;
            IRange AttndCellRange = eCells["A" + cntUsedRange];
            AttndCellRange.EntireRow.Font.Bold = true;

            AttndCellRange.CopyFromDataTable(dtEmpAttnd, SpreadsheetGear.Data.SetDataFlags.None);

            int lastRowCount = eWorksheet.UsedRange.RowCount;

            if (dtEmpAttnd == null || dtEmpAttnd.Rows == null || dtEmpAttnd.Rows.Count == 0)
            {
                lastRowCount = lastRowCount + 1;
                rngNoRecords = eCells[$"A{lastRowCount}:P{lastRowCount}"];
                rngNoRecords.Merge();
                rngNoRecords.Value = "No Records Found.";
                rngNoRecords.EntireRow.Font.Bold = true;
            }
            IR1 = eCells[$"A{cntUsedRange}:P{lastRowCount}"];
            SetBorders(IR1);
        }

        private static void SetBorders(IRange IR1)
        {
            IBorder hBorder = IR1.Borders[BordersIndex.InsideHorizontal];
            IBorder vBorder = IR1.Borders[BordersIndex.InsideVertical];
            IBorder lBorder = IR1.Borders[BordersIndex.EdgeLeft];
            IBorder rBorder = IR1.Borders[BordersIndex.EdgeRight];
            IBorder bBorder = IR1.Borders[BordersIndex.EdgeBottom];
            IBorder tBorder = IR1.Borders[BordersIndex.EdgeTop];

            hBorder.LineStyle = LineStyle.Continuous;
            vBorder.LineStyle = LineStyle.Continuous;
            lBorder.LineStyle = LineStyle.Continuous;
            rBorder.LineStyle = LineStyle.Continuous;
            bBorder.LineStyle = LineStyle.Continous;
            tBorder.LineStyle = LineStyle.Continous;
        }

        private static void SendMail(string rptStartDateString, string rptEndDateString, UnitOfWork uow, EmailTemplate temp, string fileXML, GroupReportDM aGrpRptDm)
        {
            string email_body = temp.template_body;

            if (string.IsNullOrEmpty(aGrpRptDm.toEmails))
            {
                throw new Exception("Recipient To Email ID not found.");
            }

            email_body = email_body.Replace("#emp_name#", aGrpRptDm.toNames);
            email_body = email_body.Replace("#monday#", rptStartDateString);
            email_body = email_body.Replace("#sunday#", rptEndDateString);

            bool emailSent = uow.EmailRepo.SendEmail(99999, temp.template_syscode, temp.from_email_display, temp.from_email_id, aGrpRptDm.toEmails, aGrpRptDm.ccEmails ?? string.Empty, temp.template_subject, email_body, fileXML);

            if (!emailSent)
            {
                Console.WriteLine("eMail could not be sent.");
            }
            else
            {
                Console.WriteLine("Email successfully sent.");
            }
        }

        private IList<GroupMember_TaskDtlsDTO> GetMemberTaskDtls()
        {
            IList<GroupMember_TaskDtlsDTO> lstMemTaskDtls = null;

            try
            {
                using (var uow = new UnitOfWork())
                {
                    lstMemTaskDtls = uow.SchReportsRepo.proc_get_GroupMember_TaskDtls();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return lstMemTaskDtls;
        }
    }
}
