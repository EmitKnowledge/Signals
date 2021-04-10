```
CREATE TABLE [dbo].[File](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[Path] [nvarchar](max) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[IsEncrypted] [bit] NOT NULL,
	[Data] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_File] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
ALTER TABLE [dbo].[File] ADD  CONSTRAINT [DF_File_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
ALTER TABLE [dbo].[File] ADD  CONSTRAINT [DF_File_IsEncrypted]  DEFAULT ((0)) FOR [IsEncrypted]
```