IF OBJECT_ID('System.RelationshipObject') IS NULL
    CREATE TABLE [System].[RelationshipObject](
        [RelationshipId] [uniqueidentifier] NOT NULL,
        [Object1Id] [uniqueidentifier] NOT NULL,
        [Object2Id] [uniqueidentifier] NOT NULL,

        [Name] [nvarchar](128) NOT NULL,
        [Description] [nvarchar](512) NOT NULL,

        [ReverseName] [nvarchar](128) NOT NULL,
        [ReverseDescription] [nvarchar](512) NOT NULL,

        [DateCreated] [datetime] NOT NULL,
        [CreatedBy] [uniqueidentifier] NOT NULL,
        [DateModified] [datetime] NOT NULL,
        [ModifiedBy] [uniqueidentifier] NOT NULL,
        [IsActive] [bit] NOT NULL,
     CONSTRAINT [PK_RelationshipObject] PRIMARY KEY CLUSTERED 
    (
        [RelationshipId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
GO

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_RelationshipObject_Object' AND Type = 'F') BEGIN
    ALTER TABLE [System].[RelationshipObject]
        ADD CONSTRAINT FK_RelationshipObject_Object
            FOREIGN KEY (RelationshipId) REFERENCES [System].[Object](ObjectId);
END

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_RelationshipObject_Object_Object1' AND Type = 'F') BEGIN
    ALTER TABLE [System].[RelationshipObject]
        ADD CONSTRAINT FK_RelationshipObject_Object_Object1
            FOREIGN KEY (Object1Id) REFERENCES [System].[Object](ObjectId);
END

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_RelationshipObject_Object_Object2' AND Type = 'F') BEGIN
   ALTER TABLE [System].[RelationshipObject]
        ADD CONSTRAINT FK_RelationshipObject_Object_Object2
            FOREIGN KEY (Object2Id) REFERENCES [System].[Object](ObjectId);
END