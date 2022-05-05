insert into task_menu_master(menu_syscode, menu_name, menu_description, parent_menu_syscode, page_url, is_enabled, display_order, icon)
values(20, 'Task Report', 'Task Report', 2, '/Tasks/Task/TaskReport', 1, 46, 'metismenu-icon pe-7s-note2')

insert into task_access_group_menu_details(access_group_syscode, menu_syscode)
values(1,20),(2,20)
