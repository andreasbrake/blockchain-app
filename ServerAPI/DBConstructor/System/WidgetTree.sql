IF OBJECT_ID('System.WidgetTree') IS NULL
    CREATE TABLE [System].[WidgetTree](
        [WidgetTreeId] [uniqueidentifier] NOT NULL,
        [WidgetId] [uniqueidentifier] NOT NULL,
        [ChildWidgetId] [uniqueidentifier] NOT NULL,
        [Sequence] [int] NOT NULL,

        [DateCreated] [datetime] NOT NULL,
        [CreatedBy] [uniqueidentifier] NOT NULL,
        [DateModified] [datetime] NOT NULL,
        [ModifiedBy] [uniqueidentifier] NOT NULL,
        [IsActive] [bit] NOT NULL
    CONSTRAINT [PK_WidgetTree] PRIMARY KEY CLUSTERED 
    (
        [WidgetTreeId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_WidgetTree_Widget' AND Type = 'F') BEGIN
    ALTER TABLE [System].[WidgetTree]
        ADD CONSTRAINT FK_WidgetTree_Widget
            FOREIGN KEY (WidgetId) REFERENCES [System].[Widget](WidgetId);
END

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_WidgetTree_ChildWidget' AND Type = 'F') BEGIN
    ALTER TABLE [System].[WidgetTree]
        ADD CONSTRAINT FK_WidgetTree_ChildWidget
            FOREIGN KEY (ChildWidgetId) REFERENCES [System].[Widget](WidgetId);
END

