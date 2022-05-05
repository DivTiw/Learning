/****** Object:  StoredProcedure [dbo].[proc_get_GroupMember_TaskDtls]    Script Date: 7/15/2021 11:53:27 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--exec proc_get_GroupMember_TaskDtls
ALTER procedure [dbo].[proc_get_GroupMember_TaskDtls]
AS

		DECLARE @RptSpan INT =7; --Days
		DECLARE @Today00_00 DATETIME = CAST(GETDATE() AS DATE);
		DECLARE @RptStartDate DATETIME = @Today00_00 - @RptSpan;

	SELECT	(select CAST(N'' AS XML).value(
						  'xs:base64Binary(xs:hexBinary(sql:column("bin")))'
						, 'VARCHAR(MAX)'
					)  
					FROM 
					(
						SELECT cast(CAST( URec.task_syscode as varchar) AS VARBINARY(MAX)) AS bin
					) AS bin_sql_server_temp) [encrypted_task_syscode], Urec.	employee_name
				,  URec.employee_syscode, URec.task_syscode, URec.task_reference,   URec.task_owner
				, URec.task_on_behalf, URec.created_by, URec.group_syscode
				,ISNULL(URec.project_name, '') [project_name],ISNULL(URec.module_name, '') [module_name]
				,URec.Parent_Subject,URec.task_subject,URec.status_name
				, Cast(ISNULL(SUM(URec.Hours_Worked), 0) AS DECIMAL(10,2)) [Total_Hours_Worked]
				, CAST(ISNULL(SUM(URec.Hours_Worked), 0) / 8 AS DECIMAL(10,2)) [Total_Days_Worked]
				
		FROM 
		(
			SELECT	 gm.employee_syscode, r.record_syscode, t.task_reference, t.task_subject, ts.status_name, m.module_name, t.task_owner
					, t.task_on_behalf, t.created_by, t.task_syscode, p.project_name, p.group_syscode, r.start_time, r.stop_time
					, DateDiff(SECOND,r.start_time, r.stop_time)/3600.0 as [Hours_Worked]
					, parent.task_subject as [Parent_Subject], vw.employee_name
					--, DateDiff(SECOND,r.start_time, r.stop_time)/3600.0/8 as [Days_Worked]		
			FROM (SELECT DISTINCT employee_syscode FROM group_members WHERE employee_syscode <> 3986 AND group_syscode = 1 AND is_active = 1 AND is_deleted = 0) gm
			LEFT JOIN task_user_record r on r.employee_syscode = GM.employee_syscode AND r.start_time >= @RptStartDate
			LEFT JOIN task_master t on r.task_syscode = t.task_syscode
			LEFT JOIN task_master parent on parent.task_syscode = t.parent_task_syscode
			LEFT JOIN task_status_master ts on t.task_status_syscode = ts.status_syscode
			LEFT JOIN module_master m on t.module_syscode = m.module_syscode
			LEFT JOIN project_master p on m.project_syscode = p.project_syscode	
			inner JOIN vw_employee_master vw on vw.employee_syscode = gm.employee_syscode			
		)URec
		GROUP BY URec.employee_syscode, URec.Parent_Subject, URec.task_reference, URec.task_subject, URec.task_syscode, URec.status_name, URec.task_owner
				, URec.task_on_behalf, URec.created_by, URec.module_name, URec.project_name, URec.group_syscode,URec.employee_name