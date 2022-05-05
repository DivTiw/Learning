
ALTER PROCEDURE [dbo].[proc_stopTask]
@record_syscode int,
@return_value  varchar(1000) OUTPUT
AS

	DECLARE @mToday AS DATETIME = GETDATE();

BEGIN TRY
	UPDATE task_user_record
	SET stop_time = @mToday
	WHERE record_syscode = @record_syscode

	SET @return_value = 'success'
END TRY

BEGIN CATCH
	
	DECLARE @Err AS VARCHAR(MAX) = ERROR_MESSAGE();
	SET @return_value = @Err
	EXEC [proc_save_error_log]  @Err,'[proc_stopTask]', 0
END CATCH