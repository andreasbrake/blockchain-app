IF OBJECT_ID('System.PageWidget') IS NULL
    CREATE TABLE [System].[PageWidget](
        [PageWidgetId] [uniqueidentifier] NOT NULL,

        [PageId] [uniqueidentifier] NOT NULL,
        [WidgetId] [uniqueidentifier] NOT NULL,
        [RelationshipId] [uniqueidentifier] NULL,

        [DateCreated] [datetime] NOT NULL,
        [CreatedBy] [uniqueidentifier] NOT NULL,
        [DateModified] [datetime] NOT NULL,
        [ModifiedBy] [uniqueidentifier] NOT NULL,
        [IsActive] [bit] NOT NULL
    CONSTRAINT [PK_PageWidget] PRIMARY KEY CLUSTERED 
    (
        [PageWidgetId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_PageWidget_Page' AND Type = 'F') BEGIN
    ALTER TABLE [System].[PageWidget]
        ADD CONSTRAINT FK_PageWidget_Page
            FOREIGN KEY (PageId) REFERENCES [System].[Page](PageId);
END

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_PageWidget_Widget' AND Type = 'F') BEGIN
    ALTER TABLE [System].[PageWidget]
        ADD CONSTRAINT FK_PageWidget_Widget
            FOREIGN KEY (WidgetId) REFERENCES [System].[Widget](WidgetId);
END

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_PageWidget_Relationship' AND Type = 'F') BEGIN
    ALTER TABLE [System].[PageWidget]
        ADD CONSTRAINT FK_PageWidget_Relationship
            FOREIGN KEY (RelationshipId) REFERENCES [System].[RelationshipObject](RelationshipId);
END



