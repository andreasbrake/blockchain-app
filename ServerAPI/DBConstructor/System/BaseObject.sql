IF OBJECT_ID('dbo.BaseObject') IS NULL
    CREATE TABLE [dbo].[BaseObject](
        [BaseObjectId] [uniqueidentifier] NOT NULL,
        [ModuleId] [uniqueidentifier] NOT NULL,
        [ObjectId] [uniqueidentifier] NOT NULL,

        [BaseObjectName] [nvarchar](128) NOT NULL,
        [BaseObjectIndex] [nvarchar](max) NOT NULL,

        [DateCreated] [datetime] NOT NULL,
        [CreatedBy] [uniqueidentifier] NOT NULL,
        [DateModified] [datetime] NOT NULL,
        [ModifiedBy] [uniqueidentifier] NOT NULL,
        [IsActive] [bit] NOT NULL,
    CONSTRAINT [PK_Object] PRIMARY KEY CLUSTERED 
    (
        [BaseObjectId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
GO

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_BaseObject_Module' AND Type = 'F') BEGIN
    ALTER TABLE [dbo].[BaseObject]
        ADD CONSTRAINT FK_BaseObject_Module
            FOREIGN KEY (ModuleId) REFERENCES [System].[Module](ModuleId);
END

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_BaseObject_Object' AND Type = 'F') BEGIN
    ALTER TABLE [dbo].[BaseObject]
        ADD CONSTRAINT FK_BaseObject_Object
            FOREIGN KEY (ObjectId) REFERENCES [System].[Object](ObjectId);
END
