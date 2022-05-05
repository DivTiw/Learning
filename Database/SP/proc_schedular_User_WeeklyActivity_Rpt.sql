GO
/****** Object:  StoredProcedure [dbo].[proc_schedular_User_WeeklyActivity_Rpt]    Script Date: 2/1/2021 11:55:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


--exec [dbo].[proc_schedular_User_WeeklyActivity_Rpt]
ALTER PROCEDURE [dbo].[proc_schedular_User_WeeklyActivity_Rpt]
(
	@iCreatedBy INT = 99999
)
AS
BEGIN	
		
		DECLARE @Err AS VARCHAR(MAX) = '';
		DECLARE @Today AS DATETIME = GETDATE();
		DECLARE @AppURL VARCHAR(2000);
		DECLARE @link VARCHAR(2000);
		DECLARE @encrypted_task_syscode VARCHAR(500) = '';
		DECLARE @email_id VARCHAR(500) = '';
		DECLARE @return_value VARCHAR(500) = '';

		DECLARE @RptSpan INT = 7; --Days
		DECLARE @Today00_00 DATETIME = CAST(GETDATE() AS DATE);
		DECLARE @RptStartDate DATETIME = @Today00_00 - @RptSpan;
		DECLARE @RptEndDate DATETIME = @Today00_00 - 1
		DECLARE @RptStartDateString VARCHAR(max) = DATENAME(dw,GETDATE()-@RptSpan)+ ', ' + CONVERT(VARCHAR(11),getdate()-@RptSpan,106);
		DECLARE @RptEndDateString VARCHAR(max) = DATENAME(dw,GETDATE()-1)+ ', ' + CONVERT(VARCHAR(11),getdate()-1,106);
		DECLARE @TotalHours DECIMAL(10,2) = 0, @TotalDays DECIMAL(10,2) = 0

		DECLARE
		@emp_syscode INT,
		@task_owner INT, 
		@task_onBehalf INT, 
		@task_creator INT, 
		@group_syscode INT,
		@sr_no INT

		DECLARE 
		@user_syscode int,
		@template_syscode int, 
		@template_syscode1 int, 
		@from_email_display varchar(100),
		@from_email_id varchar(100),
		@to_email_id varchar(2000),
		@cc_email_id varchar(2000),
		@subject varchar(500),
		@body varchar(max),
		@sent_on datetime,
		@created_by int ,
		@created_on datetime = @Today,
		@retry_count int,
		@status varchar(50),
		@error_flag varchar(100),
		@error_desc varchar(500),
		@project_syscode int,
		@email_return_value varchar(500), 
		@employee_name varchar(200),
		@EmailBody varchar(max),
		@EmpRefNo varchar(20),
		@Count INT = 0,
		@ErroneousEmailIds VARCHAR(MAX),
		@OutBox_Syscode INT = 0
					
		BEGIN TRY

		CREATE TABLE #AttndcDailyData
		(
			[SR No] INT, [Emp Code] INT, [Employee Name] VARCHAR(1000),	[Company Name] VARCHAR(1000),
			[Department] VARCHAR(1000), [Location] VARCHAR(1000), [Sub Location] VARCHAR(1000),	[Date] VARCHAR(1000),
			[Day] VARCHAR(1000), [Actual In Time] VARCHAR(1000), [Actual Out Time] VARCHAR(1000),	
			[Regularized In Time] VARCHAR(1000), [Regularized Out Time] VARCHAR(1000),	[Attendance Description] VARCHAR(1000),
			[Leave Type] VARCHAR(1000),	[Attendance Source] VARCHAR(1000)
		)


		SET @created_on = @Today
		SET @email_return_value = ''
		
		SELECT	(select CAST(N'' AS XML).value(
						  'xs:base64Binary(xs:hexBinary(sql:column("bin")))'
						, 'VARCHAR(MAX)'
					)  
					FROM 
					(
						SELECT cast(CAST( URec.task_syscode as varchar) AS VARBINARY(MAX)) AS bin
					) AS bin_sql_server_temp) [encrypted_task_syscode]
				,  URec.employee_syscode, URec.task_syscode, URec.task_reference, URec.task_subject, URec.status_name, URec.task_owner
				, URec.task_on_behalf, URec.created_by, ISNULL(URec.module_name, '') [module_name], ISNULL(URec.project_name, '') [project_name]
				, URec.group_syscode, Cast(ISNULL(SUM(URec.Hours_Worked), 0) AS DECIMAL(10,2)) [Total Hours Worked]
				, CAST(ISNULL(SUM(URec.Hours_Worked), 0) / 8 AS DECIMAL(10,2)) [Total Days Worked]
				, URec.Parent_Subject
		INTO #TempData		
		FROM 
		(
			SELECT	 gm.employee_syscode, r.record_syscode, t.task_reference, t.task_subject, ts.status_name, m.module_name, t.task_owner
					, t.task_on_behalf, t.created_by, t.task_syscode, p.project_name, p.group_syscode, r.start_time, r.stop_time
					, DateDiff(SECOND,r.start_time, r.stop_time)/3600.0 as [Hours_Worked]
					, parent.task_subject as [Parent_Subject]
					--, DateDiff(SECOND,r.start_time, r.stop_time)/3600.0/8 as [Days_Worked]		
			FROM (SELECT DISTINCT employee_syscode FROM group_members WHERE group_syscode = 1 AND is_active = 1 AND is_deleted = 0) gm
			LEFT JOIN task_user_record r on r.employee_syscode = GM.employee_syscode AND r.start_time >= @RptStartDate
			LEFT JOIN task_master t on r.task_syscode = t.task_syscode
			LEFT JOIN task_master parent on parent.task_syscode = t.parent_task_syscode
			LEFT JOIN task_status_master ts on t.task_status_syscode = ts.status_syscode
			LEFT JOIN module_master m on t.module_syscode = m.module_syscode
			LEFT JOIN project_master p on m.project_syscode = p.project_syscode					
		)URec
		GROUP BY URec.employee_syscode, URec.Parent_Subject, URec.task_reference, URec.task_subject, URec.task_syscode, URec.status_name, URec.task_owner
				, URec.task_on_behalf, URec.created_by, URec.module_name, URec.project_name, URec.group_syscode

		IF EXISTS(SELECT * FROM #TempData)
		BEGIN
			SELECT  @template_syscode = template_syscode,
					@subject = template_subject, 
					@body = template_body,
					@from_email_display = from_email_display,
					@from_email_id = from_email_id,
					@AppURL = link_url
			FROM email_template
			WHERE template_name = 'Sch_UsrWklyActivity_Rpt' and is_active = 1

		DECLARE cur CURSOR FOR  (SELECT DISTINCT employee_syscode FROM #TempData)
		OPEN cur
		FETCH NEXT FROM cur INTO @emp_syscode
		
		WHILE @@FETCH_STATUS = 0  
		BEGIN
			declare @rows varchar(max) = ''

			SELECT @employee_name = employee_name, @email_id = email_id
			FROM vw_employee_master
			where employee_syscode = @emp_syscode

			SELECT @TotalHours = SUM([Total Hours Worked]), @TotalDays = SUM([Total Days Worked])
			FROM #TempData		
			WHERE employee_syscode = @emp_syscode	

			IF(@TotalHours > 0 AND @TotalDays > 0)
			BEGIN
					SELECT  @rows = @rows + abc.[row]  from (
					select	'<tr><td>'
							+ ISNULL(project_name, '') COLLATE SQL_Latin1_General_CP1_CI_AS +'</td><td>'
							+ ISNULL(module_name, '') COLLATE SQL_Latin1_General_CP1_CI_AS+'</td><td>'
							+ ISNULL(Parent_Subject, '')+'</td><td><a href="'+@AppURL + [encrypted_task_syscode] + '">'
							+ ISNULL(task_subject, '') COLLATE SQL_Latin1_General_CP1_CI_AS+'</a></td><td>'
							+ ISNULL(status_name, '') COLLATE SQL_Latin1_General_CP1_CI_AS+'</td><td>'
							+ convert(varchar,[Total Hours Worked]) COLLATE SQL_Latin1_General_CP1_CI_AS+'</td><td>'
							+ convert(varchar,[Total Days Worked]) COLLATE SQL_Latin1_General_CP1_CI_AS +'</td></tr>' AS [row]
					from #TempData
					where employee_syscode = @emp_syscode) abc

					SET @rows = @rows + '<tr>
											<td></td><td></td><td></td><td></td>
											<td><b>Total</b></td>
											<td>'+ convert(varchar,@TotalHours) COLLATE SQL_Latin1_General_CP1_CI_AS+'</td>
											<td>'+ convert(varchar,@TotalDays) COLLATE SQL_Latin1_General_CP1_CI_AS+'</td>
										 </tr>'
			END
			ELSE
			BEGIN
					SET @rows = @rows + '<tr><td colspan="7">No records found</td></tr>'
			END								
			
			BEGIN --Attendance Data Fetch and replacement in the body 
				DELETE FROM #AttndcDailyData
				Insert into #AttndcDailyData		
				( [SR No] , [Emp Code] , [Employee Name] , [Company Name] , [Department], [Location], [Sub Location], [Date] , [Day] , [Actual In Time] 
				, [Actual Out Time] ,[Regularized In Time] , [Regularized Out Time] , [Attendance Description] , [Leave Type] ,	[Attendance Source] 
				)					
					EXEC proc_LMS_GetDailyAttendanceReport
					@employee_syscode = @iCreatedBy,
					@PageCalledFor = NULL,
					@PageTypeFor = NULL,
					@search_employee_syscodes = @emp_syscode,
					@search_first_date = @RptStartDate,
					@search_last_date = @RptEndDate,
					@search_company_syscode = NULL,
					@search_department_syscode = NULL,
					@search_location_syscode = NULL,
					@search_sub_location_syscode = NULL,
					@return_value = @return_value OUTPUT
		
		declare @rows1 varchar(max) = ''				
			
			SELECT  @rows1 = @rows1 + att.[row]  from (
					select	'<tr><td>'+ CONVERT(VARCHAR,[Emp Code])+'</td><td>'
							+ [Employee Name] +'</td><td>'
							+ [Day] COLLATE SQL_Latin1_General_CP1_CI_AS+'</td><td>'
							+ [Date] COLLATE SQL_Latin1_General_CP1_CI_AS+'</td><td>'							
							+ ISNULL([Actual In Time], '')  COLLATE SQL_Latin1_General_CP1_CI_AS+'</td><td>'
							+ ISNULL([Actual Out Time], '') COLLATE SQL_Latin1_General_CP1_CI_AS+'</td><td>'
							+ ISNULL([Regularized In Time], '') COLLATE SQL_Latin1_General_CP1_CI_AS +'</td><td>'
							+ ISNULL([Regularized Out Time], '') COLLATE SQL_Latin1_General_CP1_CI_AS +'</td><td>'
							+ CONVERT(VARCHAR,DATEDIFF(HOUR, CONVERT(DATETIME,COALESCE([Actual In Time], [Regularized In Time], '')), CONVERT(DATETIME,COALESCE([Actual Out Time], [Regularized Out Time], ''))))  COLLATE SQL_Latin1_General_CP1_CI_AS +'</td><td>'
							+ ISNULL([Attendance Description], '') COLLATE SQL_Latin1_General_CP1_CI_AS +'</td><td>'
							+ ISNULL([Leave Type], '') COLLATE SQL_Latin1_General_CP1_CI_AS +'</td><td>'							
							+ ISNULL([Attendance Source], '')  COLLATE SQL_Latin1_General_CP1_CI_AS +'</td></tr>' 
							AS [row]
					from #AttndcDailyData
					) att
		select @rows1		

		END

		SET @EmailBody = @body		
		SET @EmailBody = replace(@EmailBody, '#emp_name#', @employee_name)
		SET @EmailBody = replace(@EmailBody, '#monday#', @RptStartDateString)
		SET @EmailBody = replace(@EmailBody, '#sunday#', @RptEndDateString)
		SET @EmailBody = replace(@EmailBody, '#rows#', @rows)
		SET @EmailBody = replace(@EmailBody, '#rows1#', @rows1)

		SET @cc_email_id = null
		SET @retry_count = 0
		SET @status = 0
		SET @to_email_id = @email_id			

		--SELECT @EmailBody

			SELECT DISTINCT email_id
			INTO #CCUsers
			FROM vw_employee_master
			WHERE --employee_syscode in (SELECT s.val FROM [dbo].[Split](',', @TaskCCUsers) s)
				  employee_syscode in (	SELECT employee_syscode 
										FROM group_members 
										WHERE 
											group_syscode = 1 --Temporary we are hardcoding the group as only one group in live(SELECT DISTINCT group_syscode FROM group_members WHERE employee_syscode = @emp_syscode)
										AND	is_active = 1
										AND is_deleted = 0
										AND role_syscode = (SELECT role_syscode FROM task_user_role_master WHERE role_code = 'GRH')
									   )

			IF EXISTS(SELECT * FROM #CCUsers)
			BEGIN
				SELECT @cc_email_id = COALESCE(@cc_email_id + ',', '') + email_id
				FROM #CCUsers
			END

			DROP TABLE #CCUsers

			select @project_syscode = project_syscode
			from vw_Project_master
			where project_name = 'Stride'

			IF(@EmailBody IS NULL)
			BEGIN						
				THROW 50005, N'Error: Email Body is null', 1;	
			END
			
			exec  proc_CommonEmail_Send_Mail
					@emp_syscode,
					@template_syscode, 
					@from_email_display,
					@from_email_id ,
					@to_email_id ,
					@cc_email_id ,
					@subject ,
					@EmailBody,		
					@iCreatedBy,
					@created_on,
					@retry_count,
					@status ,	
					@project_syscode ,
					@email_return_value OUTPUT,
					@OutBox_Syscode OUTPUT	

		--SELECT @email_return_value
			IF(len(@email_return_value) > 0 AND @email_return_value != '#SUCCESS#')
			BEGIN					
				SET @ErroneousEmailIds = @ErroneousEmailIds + @to_email_id + ', '
			END	
				FETCH NEXT FROM cur INTO @emp_syscode
		END
		CLOSE cur
		DEALLOCATE cur
		--[END] of Loop on records

		DROP TABLE #TempData
		DROP TABLE #AttndcDailyData

		IF(@@ERROR > 0)
		BEGIN
			SET @Err = 'Error sending task stopped mail to '+ @ErroneousEmailIds	
			exec [proc_save_error_log]  @Err,'[proc_schedular_User_WeeklyActivity_Rpt]', @iCreatedBy		

			SELECT @Err
		END
	END

	END TRY
BEGIN CATCH	
	IF OBJECT_ID(N'tempdb..#TempData') IS NOT NULL
	BEGIN
	DROP TABLE #TempData
	DROP TABLE #AttndcDailyData
	END

	SET @Err = ERROR_MESSAGE();
	exec [proc_save_error_log]  @Err,'[proc_schedular_User_WeeklyActivity_Rpt]', @iCreatedBy

	SELECT @Err
	IF CURSOR_STATUS('global','cur')>=-1
	BEGIN
 	 CLOSE cur
	 DEALLOCATE cur
	END 	
END CATCH
END
