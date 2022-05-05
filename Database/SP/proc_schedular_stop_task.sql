

--exec proc_schedular_stop_task
ALTER PROCEDURE [dbo].[proc_schedular_stop_task]
(
	@iCreatedBy INT = 0
)
AS
BEGIN	
		
		DECLARE @Err AS VARCHAR(MAX) = '';
		DECLARE @Today AS DATETIME = GETDATE();
		DECLARE @AppURL VARCHAR(2000);
		DECLARE @link VARCHAR(2000);
		DECLARE @task_subject VARCHAR(2000) = '';
		DECLARE @task_ref VARCHAR(100) = '';		
		DECLARE @emp_syscode INT
		DECLARE @task_syscode INT
		DECLARE @record_syscode INT
		DECLARE @encrypted_task_syscode VARCHAR(500) = '';
		DECLARE @emp_name VARCHAR(100);
		DECLARE	@email_id VARCHAR(500) = '';
		DECLARE @return_value VARCHAR(500) = ''
	

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

		SET @created_on = @Today
		SET @email_return_value = ''
		
		SELECT  r.employee_syscode , vw.employee_name, vw.email_id, t.task_reference, t.task_subject,t.task_syscode, r.record_syscode
		INTO #TempData			
		FROM task_user_record r
		INNER JOIN task_master t on r.task_syscode = t.task_syscode
		INNER JOIN vw_employee_master vw on r.employee_syscode = vw.employee_syscode
		WHERE stop_time is null

			--SELECT * FROM #TempData

		IF EXISTS(SELECT * FROM #TempData)
		BEGIN
			SELECT  @template_syscode = template_syscode,
					@subject = template_subject, 
					@body = template_body,
					@from_email_display = from_email_display,
					@from_email_id = from_email_id,
					@AppURL = link_url
			FROM email_template
			WHERE template_name = 'Sch_StopTask' and is_active = 1
			
			--[START] of Loop on records
		DECLARE cur CURSOR FOR  (SELECT * FROM #TempData where email_id <> '')
		OPEN cur
		FETCH NEXT FROM cur INTO @emp_syscode, @emp_name, @email_id, @task_ref, @task_subject,@task_syscode,@record_syscode
		
		WHILE @@FETCH_STATUS = 0  
		BEGIN
			
			EXEC proc_stopTask @record_syscode, @return_value OUTPUT

			IF(@return_value <> 'success')
			BEGIN		
					 SET @Err = 'Error occurred in proc_stopTask for record' + @record_syscode;
					THROW 50005, @Err, 1;			
			END

			SELECT @encrypted_task_syscode = 
					CAST(N'' AS XML).value(
						  'xs:base64Binary(xs:hexBinary(sql:column("bin")))'
						, 'VARCHAR(MAX)'
					)  
					FROM 
					(
						SELECT cast(CAST( @task_syscode as varchar) AS VARBINARY(MAX)) AS bin
					) AS bin_sql_server_temp; 

			SET @EmailBody = @body		

			SET @link= @AppURL + @encrypted_task_syscode

			SET @EmailBody = replace(@EmailBody, '#emp_name#', @emp_name)
			SET @EmailBody = replace(@EmailBody, '#ref#', @task_ref)
			SET @EmailBody = replace(@EmailBody, '#subject#', @task_subject)
			SET @EmailBody = replace(@EmailBody, '#Link#', @link)

			SET @cc_email_id = '' 		
			SET @retry_count = 0
			SET @status = 0
			SET @to_email_id = @email_id

			select @project_syscode = project_syscode
			from vw_Project_master
			where project_name = 'Stride'

			IF(@EmailBody IS NULL)
			BEGIN						
				THROW 50005, N'Error: Email Body is null', 1;	
			END

			--SEND MAIL 
			exec proc_CommonEmail_Send_Mail
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

		
			IF(len(@email_return_value) > 0 AND @email_return_value != '#SUCCESS#')
			BEGIN					
				SET @ErroneousEmailIds = @ErroneousEmailIds + @to_email_id + ', '
			END	
				FETCH NEXT FROM cur INTO @emp_syscode, @emp_name, @email_id, @task_ref, @task_subject,@task_syscode,@record_syscode
		END
		CLOSE cur
		DEALLOCATE cur
		--[END] of Loop on records

		DROP TABLE #TempData

		IF(@@ERROR > 0)
		BEGIN
			SET @Err = 'Error sending task stopped mail to '+ @ErroneousEmailIds	
			exec [proc_save_error_log]  @Err,'[proc_schedular_stop_task]', @iCreatedBy		

			SELECT @Err
		END
	END

		END TRY
BEGIN CATCH

	IF OBJECT_ID(N'tempdb..#TempData') IS NOT NULL
	BEGIN
	DROP TABLE #TempData
	END

	SET @Err = ERROR_MESSAGE();
	exec [proc_save_error_log]  @Err,'[proc_schedular_stop_task]', @iCreatedBy

	SELECT @Err
END CATCH
END


