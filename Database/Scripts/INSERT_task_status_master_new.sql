insert into task_status_master (status_syscode,status_name, status_icon)
values (6,'Initiate', 'badge badge-pill badge-info'),
(7,'Acknowledge', 'badge badge-pill badge-warning'),
(8,'OnHold','badge badge-pill badge-danger'),
(9,'Discard','badge badge-pill badge-danger')

Delete from task_status_master
where status_name in ('Invalid', 'Inactive')

update task_master
set task_status_syscode = 9
where task_status_syscode = 2