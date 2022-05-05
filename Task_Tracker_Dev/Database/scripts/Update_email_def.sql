update email_definitions
set template_syscode = 2
where email_type_syscode = 8
and task_type_syscode is null
and status_syscode is null

update email_template
set template_name = 'Inform_To'
where template_name = 'Other_Updates'