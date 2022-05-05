CREATE TABLE [dbo].[module_master]
(
	module_syscode int primary key identity(1,1) not null,
	module_name varchar(100) not null,
	project_syscode int foreign key references [dbo].[project_master](project_syscode),
	workflow_syscode int references [dbo].[workflow_master](workflow_syscode), 
	created_by int,
	created_on datetime,
	modified_by int,
	modified_on datetime,
	is_active bit,
	is_deleted bit,
	CONSTRAINT [UQC_Modules] UNIQUE NONCLUSTERED(module_name, project_syscode)
);
GO