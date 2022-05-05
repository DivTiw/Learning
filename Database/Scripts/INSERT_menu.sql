

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
           (18
           ,'Create Project Task'
           ,'Create Project Task'
           ,2
           ,'/Tasks/ProjectTask/CreateProjectTask'
           ,1
           ,42
           ,'metismenu-icon pe-7s-plus')
GO


INSERT INTO [dbo].[task_access_group_menu_details]
           ([access_group_syscode]
           ,[menu_syscode])
     VALUES (1,18), (2,18)
GO
