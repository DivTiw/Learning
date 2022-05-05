--******* Update Queries for Phase - 2 Development ******--
USE[task_tracker_build]

update task_status_master
set status_name = 'Invalid'
where status_name = 'Closed'