insert into task_menu_master(menu_syscode, menu_name, menu_description, parent_menu_syscode, page_url, is_enabled, display_order, icon)
values(17, 'Group List', 'Group List', 8, '/Master/Group/Index', 1, 24, 'metismenu-icon pe-7s-menu')

insert into task_access_group_menu_details(access_group_syscode, menu_syscode)
values(1, 17)

insert into task_access_group_menu_details(access_group_syscode, menu_syscode)
values(2, 17)