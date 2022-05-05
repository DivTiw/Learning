
/*
EXEC [proc_authorise_user] 'jmf\ssssram',0,output
EXEC [proc_authorise_user] 'jmf\rajans',0,output
EXEC [proc_authorise_user] '80074',0,output
EXEC [proc_authorise_user] '10281',0,output

EXEC [proc_authorise_user] 'Jmfl\Saimaf-11',0,output
*/

CREATE PROCEDURE proc_authorise_user
	@Windows_login_id varchar(100),
	@status bit output,
	@return_value varchar(1000)  output
AS
BEGIN

		SET @return_value = '';
		SET @status = 0;

		DECLARE @employee_name AS VARCHAR(200);
		DECLARE @employee_syscode AS INT = 0;
		DECLARE @user_syscode AS INT = 0;
		DECLARE @user_type_syscode AS INT = 0;
		DECLARE @user_role AS VARCHAR(200) = '';
		
	BEGIN TRY

		SELECT @employee_syscode  = employee_syscode 
		FROM vw_employee_master
		WHERE (windows_login_id = @Windows_login_id OR CAST(employee_code AS VARCHAR(50)) = @Windows_login_id)


		IF( (@employee_syscode = 0 OR @employee_syscode IS NULL)  )
		BEGIN
			SET @return_value = 'Invalid User.';
			SET @user_syscode = 0
			SET @user_type_syscode = 0
				SET @status = 0
		END

		IF(@employee_syscode > 0)
		BEGIN
			SELECT @employee_name = employee_name
			FROM vw_employee_master
			WHERE employee_syscode = @employee_syscode

			SET @status = 1
			SET @user_syscode = 9999999 -- anonymous user 
			SET @user_type_syscode = 9999999
			SET @return_value = 'Valid User';
		END
		
	
			SELECT	employee_syscode = @employee_syscode, employee_name = @employee_name, [status] = @status,
						user_syscode = @user_syscode, user_type_syscode = @user_type_syscode, user_role = @user_role, 
						return_value = @return_value
	
			INSERT INTO [task_login_history]
				   (employee_syscode)
			VALUES (@employee_syscode)
			
	END TRY
	BEGIN CATCH
		SET @status = 0
		set @return_value = 'Error occurred.'
		DECLARE @Err AS VARCHAR(MAX) = ERROR_MESSAGE();	
		exec [proc_save_error_log]  @Err,'[proc_authorise_user]', @employee_syscode
	END CATCH

END


