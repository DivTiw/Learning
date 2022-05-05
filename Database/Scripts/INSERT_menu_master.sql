USE [task_tracker_build]
GO

INSERT INTO [dbo].[task_menu_master]
           ([menu_syscode]
           ,[menu_name]
           ,[menu_description]
           ,[parent_menu_syscode]
           ,[page_url]
           ,[is_enabled]
           ,[display_order]
           ,[icon])
     VALUES
           (16
           ,'My Task Activity-Today'
           ,'My Task Activity-Today'
           ,3
           ,'/Dashboard/Dashboard/MyTaskActivity'
           ,1
           ,11
           ,'metismenu-icon pe-7s-display1')
GO



INSERT INTO [dbo].[task_access_group_menu_details]
           ([access_group_syscode]
           ,[menu_syscode])
     VALUES (1 ,16),  (2 ,16)
GO
