USE [task_tracker_build]
GO

INSERT INTO [dbo].[task_priority_master]
           ([priority_syscode],
		   [priority_name]
         )
     VALUES
           (1,'High'),
		   (2,'Medium'),
		   (3,'Low')
GO


