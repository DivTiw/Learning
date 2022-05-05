CREATE TABLE [dbo].[workflow_level_details]
(
	level_syscode int primary key identity(1,1) not null,
	level_name varchar(100),
	level_order int,
	workflow_syscode int references [dbo].[workflow_master](workflow_syscode),
	created_by int,
	created_on datetime,
	modified_by int,
	modified_on datetime,
	is_active bit,
	is_deleted bit
);
GO