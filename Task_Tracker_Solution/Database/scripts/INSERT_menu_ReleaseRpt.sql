
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
           (21
           ,'Release Report'
           ,'Release Report'
           ,2
           ,'/Reports/Report/ReleaseReport'
           ,1
           ,47
           ,'metismenu-icon pe-7s-note2')
GO


INSERT INTO [dbo].[task_access_group_menu_details]
           ([access_group_syscode]
           ,[menu_syscode])
     VALUES (1,21), (2,21)
GO