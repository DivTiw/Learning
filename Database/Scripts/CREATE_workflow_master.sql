CREATE TABLE [dbo].[workflow_master]
(
	workflow_syscode int primary key identity(1,1) not null,
	workflow_name varchar(200) unique,
	dept_name varchar(100),
	created_by int,
	created_on datetime,
	modified_by int,
	modified_on datetime,
	is_active bit,
	is_deleted bit,
	CONSTRAINT UQC_Workflow UNIQUE NONCLUSTERED(workflow_name, dept_name)
);
GO