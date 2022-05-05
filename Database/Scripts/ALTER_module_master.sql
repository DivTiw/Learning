ALTER TABLE module_master
ADD module_description varchar(2000) NULL,
category_syscode int NULL FOREIGN KEY(category_syscode) REFERENCES category_master (category_syscode)