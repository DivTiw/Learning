
CREATE TABLE [dbo].[task_user_mapping](
user_mapping_syscode		 INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
employee_syscode		int	,
user_role_syscode		int	 FOREIGN KEY REFERENCES dbo.[task_user_role_master](role_syscode),
type_detail 	VARCHAR(200),
type_syscode		INT,
created_by		INT,
created_on		DATETIME DEFAULT GETDATE(),
modified_by		INT,
modified_on		DATETIME,
is_deleted		BIT
)

