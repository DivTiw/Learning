ALTER TABLE email_type_master
ADD Constraint unq_EmailType_Code UNIQUE(code)

ALTER TABLE task_type_master
ADD Constraint unq_TaskType_Code UNIQUE(code)

ALTER TABLE email_recipient_master
ADD Constraint unq_RecipType_Code UNIQUE(code)