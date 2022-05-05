UPDATE email_definitions
SET is_active = 0, is_deleted = 1
WHERE	email_type_syscode = 7
AND		task_type_syscode = 2
AND		status_syscode in (1, 10)
AND		template_syscode = 3