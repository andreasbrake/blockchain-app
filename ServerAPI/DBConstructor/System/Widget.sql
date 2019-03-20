IF OBJECT_ID('System.Widget') IS NULL
    CREATE TABLE [System].[Widget](
        [WidgetId] [uniqueidentifier] NOT NULL,
        
        [Name] [nvarchar](256) NOT NULL,
        [WidgetType] [nvarchar](256) NOT NULL,
        [WidgetProperties] [nvarchar](max) NOT NULL,
        [Template] [nvarchar](max) NULL,
        [ModuleId] [uniqueidentifier] NOT NULL,

        [DateCreated] [datetime] NOT NULL,
        [CreatedBy] [uniqueidentifier] NOT NULL,
        [DateModified] [datetime] NOT NULL,
        [ModifiedBy] [uniqueidentifier] NOT NULL,
        [IsActive] [bit] NOT NULL
        CONSTRAINT [CHK_HasTemplateWhenCustom] CHECK
        (
            [Template] IS NOT NULL OR [WidgetType] <> 'Custom'
        ),
        CONSTRAINT [PK_Widget] PRIMARY KEY CLUSTERED 
        (
            [WidgetId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Widget_Module' AND Type = 'F') BEGIN
    ALTER TABLE [System].[Widget]
        ADD CONSTRAINT FK_Widget_Module
            FOREIGN KEY (ModuleId) REFERENCES [System].[Module](ModuleId);
END

