

CREATE TABLE [error_log_master](
	[error_syscode] [int] IDENTITY(1,1) NOT NULL,
	[error_description] [varchar](1000) NOT NULL,
	[sp_name] [varchar](150) NOT NULL,
	[on_date] [datetime] NOT NULL,
	[user_syscode] [int] NULL,
 CONSTRAINT [PK_error_log] PRIMARY KEY CLUSTERED 
(
	[error_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [error_log_master] ADD  CONSTRAINT [DF_error_log_on_date]  DEFAULT (getdate()) FOR [on_date]
GO


