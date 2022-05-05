ALTER TABLE workflow_master
ADD group_syscode int NULL FOREIGN KEY(group_syscode) REFERENCES group_master (group_syscode)



