update task_access_group_master
set group_syscode = 2
where access_group_name = 'ED' and access_group_syscode = 2

update task_menu_master
set menu_name = 'My Assigned Tasks',
menu_description = 'My Assigned Tasks'
where menu_syscode = 7 and menu_name = 'My Tasks'

update task_menu_master
set display_order = 12
where menu_name = 'Admin Dashboard' and menu_syscode = 14

insert into task_menu_master(menu_syscode, menu_name, menu_description, parent_menu_syscode, page_url, is_enabled, display_order, icon)
values(15, 'My Owned Tasks', 'My Owned Tasks', 2, '/Tasks/Task/MyOwnedTasks', 1, 44, 'metismenu-icon pe-7s-note2')

insert into task_access_group_menu_details(access_group_syscode, menu_syscode)
values(1, 15)

insert into task_access_group_menu_details(access_group_syscode, menu_syscode)
values(2, 15)