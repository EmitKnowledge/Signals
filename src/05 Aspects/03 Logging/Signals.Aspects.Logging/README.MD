## Quick start .NET Core 2.0 with NLog

---

#### Using file system for logging
```
services.AddTransient<ILogger, NLogger>((serviceProvider) =>
{
    return new NLogger(new DefaultFileConfiguration());
});
```

---

#### Using database for logging  
First create new table in database:  
```
CREATE TABLE [dbo].[LogEntry](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
	[Level] [nvarchar](60) NULL,
	[ErrorGroup] [nvarchar](60) NULL,
	[ErrorCode] [nvarchar](60) NULL,
	[Origin] [nvarchar](120) NULL,
	[Action] [nvarchar](600) NULL,
	[ActionFilePath] [nvarchar](max) NULL,
	[ActionSourceLineNumber] [nvarchar](30) NULL,
	[Message] [nvarchar](max) NULL,
	[ExceptionMessage] [nvarchar](max) NULL,
	[UserId] [int] NULL,
	[Payload] [nvarchar](max) NULL,
 CONSTRAINT [PK_LogEntry] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[LogEntry] ADD  CONSTRAINT [DF_LogEntry_CreatedOn]  DEFAULT (getdate()) FOR [CreatedOn]
ALTER TABLE [dbo].[LogEntry] WITH CHECK ADD CONSTRAINT [FK_LogEntry_User] FOREIGN KEY([UserId]) REFERENCES [dbo].[User] ([Id])
ALTER TABLE [dbo].[LogEntry] CHECK CONSTRAINT [FK_LogEntry_User]
```

Register NLogger using database configuration:
```
services.AddTransient<ILogger, NLogger>((serviceProvider) =>
{
    var config = new DefaultDatabaseConfiguration();
    config.Host = "Hostname";
    config.Database = "LogEntry";
    config.Username = "Username";
    config.Password = "Password";
    return new NLogger(new DefaultDatabaseConfiguration());
});
```

---

#### Creating custom logger

- Create ```CustomLogEntry``` that represents log entry
- Implement ```CustomLogger : ILoggerBase<T> where T : CustomLogEntry```
- Register custom logger
```
services.AddTransient<ILoggerBase<CustomLogEntry>, NLogger>((serviceProvider) =>
{
    return new NLogger(new DefaultFileConfiguration());
});
```