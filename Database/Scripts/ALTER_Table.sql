ALTER TABLE task_master
ADD module_syscode int references [dbo].[module_master](module_syscode),
	department_syscode int,
	category_syscode int references [dbo].[task_category_master](category_syscode),
	subcategory_syscode int,
	level_syscode int references [dbo].[workflow_level_details](level_syscode),
	weightage decimal;
GO

ALTER TABLE task_user_mapping
ADD is_active bit;
GO

ALTER TABLE task_trail
ADD trail_description varchar(1000),
	trail_comments varchar(2000);
GO

--ALTER TABLE project_master
--ADD CONSTRAINT UQC_Project UNIQUE NONCLUSTERED(project_name)
--GO

--ALTER TABLE workflow_master
--ADD CONSTRAINT UQC_Workflow UNIQUE NONCLUSTERED(workflow_name, dept_name)
--GO

--ALTER TABLE module_Master
--ADD CONSTRAINT [UQC_Modules] UNIQUE NONCLUSTERED(module_name, project_syscode)
--GO
