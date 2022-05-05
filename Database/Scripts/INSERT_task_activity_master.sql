USE [task_tracker_build]
GO

INSERT INTO [dbo].[task_activity_master]
           ([activity_syscode],
		   [activity_name]
         )
     VALUES
           (1,'Created'),
		   (2,'Acknowledged'),
		   (3,'Started'),
		   (4,'Updated'),
		   (5,'Created For'),
		   (6,'Forwarded'),
		   (7,'Completed'),
		   (8,'Closed')
GO
 [dbo].[task_master]

