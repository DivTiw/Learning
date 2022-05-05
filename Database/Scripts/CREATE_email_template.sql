CREATE TABLE [email_template](
	[template_syscode] [int] NOT NULL,
	[template_name] [varchar](200) NULL,
	[template_subject] [varchar](2000) NULL,
	[template_body] [varchar](max) NULL,
	[from_email_display] [varchar](200) NULL,
	[created_on] [datetime] NULL,
	[created_by] [int] NULL,
	[modified_on] [datetime] NULL,
	[modified_by] [int] NULL,
	[is_active] [bit] NULL,
	[from_email_id] [varchar](200) NULL,
	[template_header] [varchar](200) NULL,
	[template_footer] [varchar](max) NULL,
 CONSTRAINT [PK_Email_Template] PRIMARY KEY CLUSTERED 
(
	[template_syscode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [email_template] ADD  CONSTRAINT [DF_Email_Template_created_on]  DEFAULT (getdate()) FOR [created_on]
GO