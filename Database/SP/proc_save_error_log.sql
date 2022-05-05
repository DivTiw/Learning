CREATE procedure [proc_save_error_log]
@error_description varchar(1000),
@sp_name varchar(150),
@user_syscode int
as

insert into [error_log_master]
	   (error_description, sp_name, user_syscode)
values (@error_description, @sp_name, @user_syscode)