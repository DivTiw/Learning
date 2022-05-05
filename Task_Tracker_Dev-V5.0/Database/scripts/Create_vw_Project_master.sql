USE [task_tracker_build]
GO

/****** Object:  View [dbo].[vw_department_master]    Script Date: 05-03-2021 20:32:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_Project_master]
AS
SELECT *
FROM   [UATDATABASE.JMFL.COM\SQLDBSERVER2014, 1433].[common_email_uat].dbo.common_project_master;

GO


