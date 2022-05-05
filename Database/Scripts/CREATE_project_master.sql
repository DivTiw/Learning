CREATE TABLE [dbo].[project_master]
(
	project_syscode int primary key identity(1,1) not null,
	project_name varchar(500) not null,
	created_by int,
	created_on datetime,
	modified_by int,
	modified_on datetime,
	is_active bit,
	is_deleted bit,
	CONSTRAINT UQC_Project UNIQUE NONCLUSTERED(project_name)
);
Go