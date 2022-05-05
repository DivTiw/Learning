SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Divyanand Tiwari
-- Create date: 05 Mar 2021
-- Description:	This procedure will act as a bridge between LMS database for getting report on attendance.
-- =============================================
CREATE PROCEDURE proc_LMS_GetDailyAttendanceReport 
	@employee_syscode INT = NULL,
	@PageCalledFor varchar(30) = NULL,
	@PageTypeFor varchar(30) = NULL,
	@search_employee_syscodes varchar(MAX) = NULL,
	@search_first_date datetime = NULL,
	@search_last_date datetime = NULL,
	@search_company_syscode INT = NULL,
	@search_department_syscode INT = NULL,
	@search_location_syscode INT = NULL,
	@search_sub_location_syscode INT = NULL,
	@return_value varchar(1000)  output
AS
BEGIN
EXEC [UATDATABASE.JMFL.COM\SQLDBSERVER2014, 1433].[lms_uat_v2].[Attnd].[proc_Rpt_Attendance_Daily]--EXEC [lms_build].[Attnd].[proc_Rpt_Attendance_Daily] --
					@employee_syscode,
					@PageCalledFor,
					@PageTypeFor,
					@search_employee_syscodes,
					@search_first_date,
					@search_last_date,
					@search_company_syscode,
					@search_department_syscode,
					@search_location_syscode,
					@search_sub_location_syscode,
					@return_value OUTPUT
	
END
GO
