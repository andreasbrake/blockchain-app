IF OBJECT_ID('System.Page') IS NULL
    CREATE TABLE [System].[Page](
        [PageId] [uniqueidentifier] NOT NULL,

        [Name] [nvarchar](256) NOT NULL,
        [Template] [nvarchar](max) NOT NULL,
        [ModuleId] [uniqueidentifier] NOT NULL,
        [MainWidgetId] [uniqueidentifier] NULL,

        [DateCreated] [datetime] NOT NULL,
        [CreatedBy] [uniqueidentifier] NOT NULL,
        [DateModified] [datetime] NOT NULL,
        [ModifiedBy] [uniqueidentifier] NOT NULL,
        [IsActive] [bit] NOT NULL
    CONSTRAINT [PK_Page] PRIMARY KEY CLUSTERED 
    (
        [PageId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Page_Module' AND Type = 'F') BEGIN
    ALTER TABLE [System].[Page]
        ADD CONSTRAINT FK_Page_Module
            FOREIGN KEY (ModuleId) REFERENCES [System].[Module](ModuleId);
END

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Page_Widget' AND Type = 'F') BEGIN
    ALTER TABLE [System].[Page]
        ADD CONSTRAINT FK_Page_Widget
            FOREIGN KEY (MainWidgetId) REFERENCES [System].[Widget](WidgetId);
END


