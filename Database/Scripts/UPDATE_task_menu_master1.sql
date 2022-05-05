update task_menu_master
set menu_name = 'Masters',
menu_description = 'Masters'
where menu_name = 'Workflow'

update task_menu_master
set display_order = 21
where menu_name = 'Group List'

update task_menu_master
set display_order = 22
where menu_name = 'Category List'

update task_menu_master
set display_order = 23
where menu_name = 'Workflow List'

update task_menu_master
set display_order = 32	,
parent_menu_syscode = 1
where menu_name = 'Workflow Levels Mapping'
