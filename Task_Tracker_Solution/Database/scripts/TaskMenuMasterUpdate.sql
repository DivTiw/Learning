select * from task_menu_master

update task_menu_master
set page_url = REPLACE(page_url, 'Index', 'GetModuleWFLevelMap')
where menu_name = 'Workflow Levels Mapping'