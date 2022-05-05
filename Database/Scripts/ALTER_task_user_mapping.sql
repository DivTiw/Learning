ALTER TABLE task_user_mapping
ADD task_syscode int,
 trail_syscode int
 
 ALTER TABLE task_user_mapping
DROP COLUMN type_detail,type_syscode;

ALTER TABLE task_user_mapping
ADD FOREIGN KEY (trail_syscode) REFERENCES task_trail(trail_syscode); 