ALTER TABLE project_master
ADD project_description varchar(2000) NULL,
group_syscode int NULL FOREIGN KEY(group_syscode) REFERENCES group_master (group_syscode)



