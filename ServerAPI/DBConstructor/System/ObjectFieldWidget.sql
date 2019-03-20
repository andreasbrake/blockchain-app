IF OBJECT_ID('System.ObjectFieldWidget') IS NULL
    CREATE TABLE [System].[ObjectFieldWidget](
        [ObjectFieldWidgetId] [uniqueidentifier] NOT NULL,

        [ObjectFieldId] [uniqueidentifier] NOT NULL,
        [WidgetId] [uniqueidentifier] NOT NULL,

        [DateCreated] [datetime] NOT NULL,
        [CreatedBy] [uniqueidentifier] NOT NULL,
        [DateModified] [datetime] NOT NULL,
        [ModifiedBy] [uniqueidentifier] NOT NULL,
        [IsActive] [bit] NOT NULL
    CONSTRAINT [PK_ObjectFieldWidget] PRIMARY KEY CLUSTERED 
    (
        [ObjectFieldWidgetId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_ObjectFieldWidget_ObjectField' AND Type = 'F') BEGIN
    ALTER TABLE [System].[ObjectFieldWidget]
        ADD CONSTRAINT FK_ObjectFieldWidget_ObjectField
            FOREIGN KEY (ObjectFieldId) REFERENCES [System].[ObjectField](ObjectFieldId);
END


IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_ObjectFieldWidget_Widget' AND Type = 'F') BEGIN
    ALTER TABLE [System].[ObjectFieldWidget]
        ADD CONSTRAINT FK_ObjectFieldWidget_Widget
            FOREIGN KEY (WidgetId) REFERENCES [System].[Widget](WidgetId);
END

