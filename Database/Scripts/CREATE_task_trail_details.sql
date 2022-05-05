CREATE TABLE [dbo].[task_trail_details](
trail_details_syscode int PRIMARY KEY IDENTITY(1,1) NOT NULL,
trail_syscode INT	FOREIGN KEY REFERENCES [task_trail](trail_syscode),
trail_description varchar(1000),
trail_comments varchar(1000)
)
