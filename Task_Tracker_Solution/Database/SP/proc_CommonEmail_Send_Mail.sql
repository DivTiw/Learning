SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Divyanand Tiwari
-- Create date: 02 Mar 2021
-- Description:	This procedure will act as a bridge between Common Email database for Mail sending.
-- =============================================
CREATE PROCEDURE proc_CommonEmail_Send_Mail 
	@user_syscode int,
	@template_syscode int, 
	@from_email_display varchar(100),
	@from_email_id varchar(100),
	@to_email_id varchar(max),
	@cc_email_id varchar(max),
	@subject varchar(500),
	@body varchar(max),
	@created_by int ,
	@created_on datetime,
	@retry_count int,
	@status varchar(50),
	@project_syscode int,
	@return_value varchar(1000) output,
	@OutBox_Syscode int = null output,
	@tempAttachment varchar(max) = null,
	@bcc_email_id varchar(max) = null,
	@foremployee_syscodes varchar(8000) = null
AS
BEGIN
	exec  [UATDATABASE.JMFL.COM\SQLDBSERVER2014, 1433].[common_email_uat].[dbo].[proc_save_email_to_outbox] -- EXEC [common_email_build].[dbo].[proc_save_email_to_outbox]
					@user_syscode,
					@template_syscode, 
					@from_email_display,
					@from_email_id ,
					@to_email_id ,
					@cc_email_id ,
					@subject ,
					@body,		
					@created_by,
					@created_on,
					@retry_count,
					@status ,	
					@project_syscode ,
					@return_value OUTPUT,
					@OutBox_Syscode OUTPUT	
END
GO
