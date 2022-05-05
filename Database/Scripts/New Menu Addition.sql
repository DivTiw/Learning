insert into task_menu_master(menu_syscode, menu_name, menu_description, parent_menu_syscode, page_url, is_enabled, display_order, icon)
values(19, 'Team Activity', 'Details of Team Activity', 3, '/Dashboard/Dashboard/TeamActivity', 1, 13, 'metismenu-icon pe-7s-display1')

insert into task_access_group_menu_details(access_group_syscode, menu_syscode)
values(2,19)

insert into task_access_user_master(access_group_syscode, user_type_syscode, is_active, employee_syscode, remarks
, created_by, created_on)
values(2, 2, 1, <4944>, 'ED', 1, GETDATE()),
      (2, 2, 1, <7022>, 'ED', 1, GETDATE())