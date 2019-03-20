IF OBJECT_ID('System.Object') IS NULL
    CREATE TABLE [System].[Object](
        [ObjectId] [uniqueidentifier] NOT NULL,
        [Name] [nvarchar](32) NOT NULL,
        [Description] [nvarchar](512) NULL,

        [ModuleId] [uniqueidentifier] NOT NULL,
        [ParentObjectId] [uniqueidentifier] NULL,
        [MainObjectId] [uniqueidentifier] NULL,

        [IsBaseObject] [bit] NOT NULL,

        [DateCreated] [datetime] NOT NULL,
        [CreatedBy] [uniqueidentifier] NOT NULL,
        [DateModified] [datetime] NOT NULL,
        [ModifiedBy] [uniqueidentifier] NOT NULL,
        [IsActive] [bit] NOT NULL,
        CONSTRAINT [CHK_HasMainOrRelatedObject] CHECK
        (
            (
                IIF([ParentObjectId] IS NOT NULL, 1, 0) + 
                IIF([MainObjectId] IS NOT NULL, 1, 0)
            ) <= 1
        ),
        CONSTRAINT [UC_Object_Name] UNIQUE (Name, ModuleId),
        CONSTRAINT [PK_Object] PRIMARY KEY CLUSTERED 
        (
            [ObjectId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
GO

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Object_Module' AND Type = 'F') BEGIN
    ALTER TABLE [System].[Object]
        ADD CONSTRAINT FK_Object_Module
            FOREIGN KEY (ModuleId) REFERENCES [System].[Module](ModuleId);
END

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Object_Object_Parent' AND Type = 'F') BEGIN
    ALTER TABLE [System].[Object]
        ADD CONSTRAINT FK_Object_Object_Parent
            FOREIGN KEY (ParentObjectId) REFERENCES [System].[Object](ObjectId);
END

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Object_Object_Main' AND Type = 'F') BEGIN
    ALTER TABLE [System].[Object]
        ADD CONSTRAINT FK_Object_Object_Main
            FOREIGN KEY (MainObjectId) REFERENCES [System].[Object](ObjectId);
END
