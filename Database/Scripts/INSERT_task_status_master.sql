
INSERT INTO [dbo].[task_status_master]
           ([status_syscode],
		   [status_name]
         )
     VALUES
           (1,'Open'),
		   (2,'Closed'),
		   (3,'InProgress'),
		   (4,'Inactive')
GO
