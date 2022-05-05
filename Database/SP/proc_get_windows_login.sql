
CREATE Procedure [dbo].[proc_get_windows_login]
@employee_syscode int
as
Begin

Select windows_login_id,employee_code,employee_name,email_id from vw_employee_master Where employee_syscode = @employee_syscode

End