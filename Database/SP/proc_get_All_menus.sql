CREATE procedure [dbo].[proc_get_All_menus]
AS

-- For Employee
select mm.menu_syscode, mm.menu_name, mm.menu_description, mm.parent_menu_syscode, isnull(mm.page_url, '') page_url, isnull(mm.icon, '') [icon]
from task_access_group_master gm 
inner join task_access_group_menu_details agm on agm.access_group_syscode = gm.access_group_syscode
inner join task_menu_master mm on mm.menu_syscode = agm.menu_syscode and is_enabled = 1
where gm.access_group_syscode = 1
order by display_order
