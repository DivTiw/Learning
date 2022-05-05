CREATE TABLE task_login_history(
	[login_history_syscode] [int] IDENTITY(1,1) NOT NULL,
	[employee_syscode] [int] NOT NULL,
	[login_on] [datetime] NOT NULL,
	[switched_user_syscode] [int] NULL,
 CONSTRAINT [PK_login_history] PRIMARY KEY CLUSTERED 
(
	[login_history_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [task_login_history] ADD  CONSTRAINT [DF_login_history_login_on]  DEFAULT (getdate()) FOR [login_on]
GO

