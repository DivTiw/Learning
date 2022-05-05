/*Task Creation Standalone task.*/
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients)
VALUES(7, 1, 1, 1, 'TMEM', 'TCRTR, TWNR, TONBF')
/*Task Creation Workflow task.*/
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients)
VALUES(7, 2, 6, 6, 'TMEM', 'TCRTR, TWNR, TONBF')
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients)
VALUES(7, 2, 1, 3, 'TCRTR', 'TCRTR')
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients)
VALUES(7, 2, 10, 3, 'TCRTR', 'TCRTR')
/*Task Creation Workflow task Parent.*/
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients)
VALUES(13, 2, NULL, 3, 'TMEM, TWNR, TONBF', 'TCRTR')
/*Task Updation Workflow task*/
--Open Status, We don't have any templates so assigning 5
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients, is_active, is_deleted)
VALUES(8, NULL, 1, 5, 'TMEM', 'TCRTR, TWNR, TONBF', 0, 1)
--Initiated tasks
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients)
VALUES(8, NULL, 6, 6, 'TMEM', 'TCRTR, TWNR, TONBF')
--Inform To
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients)
VALUES(12, NULL, NULL, 5, 'TINFT', 'TMEM, TCRTR, TWNR, TONBF, PMWrite')
--Inprogress
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients)
VALUES(8, NULL, 3, 8, 'TMEM, TCRTR', 'TWNR, TONBF, PMWrite')
--Complete
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients)
VALUES(8, NULL, 5, 11, 'TMEM, TCRTR', 'TWNR, TONBF, PMWrite')
--Acknowledged
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients)
VALUES(8, NULL, 7, 7, 'TMEM, TCRTR', 'TWNR, TONBF, PMWrite')
--OnHold
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients)
VALUES(8, NULL, 8, 9, 'TMEM, TCRTR', 'TWNR, TONBF, PMWrite')
--Complete
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients)
VALUES(8, NULL, 9, 10, 'TMEM, TCRTR', 'TWNR, TONBF, PMWrite')
--ToDo, We don't have any templates so assigning 5
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients, is_active, is_deleted)
VALUES(8, NULL, 10, 5, 'TCRTR', 'TMEM, TWNR, TONBF', 0, 1)
--No Status changed just other updates like comments adding users etc.
INSERT INTO email_definitions (email_type_syscode, task_type_syscode, status_syscode, template_syscode, to_recipients, cc_recipients)
VALUES(8, NULL, NULL, 2, 'TMEM, TCRTR', 'TWNR, TONBF, PMWrite')

