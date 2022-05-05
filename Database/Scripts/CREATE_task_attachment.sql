CREATE TABLE [dbo].[task_attachment](
attachment_syscode		 INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
attachment_identifier	UNIQUEIDENTIFIER DEFAULT NEWID(),
return_identifier		UNIQUEIDENTIFIER DEFAULT NEWID(),
attachment_filename	VARCHAR(200),
attachment_display_name		VARCHAR(200),
type_detail 	VARCHAR(200),
type_syscode		INT,
created_by		INT,
created_on		DATETIME DEFAULT GETDATE(),
modified_by		INT,
modified_on		DATETIME,
is_deleted		BIT
)