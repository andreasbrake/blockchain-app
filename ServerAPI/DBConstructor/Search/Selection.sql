IF OBJECT_ID('Search.Selection') IS NULL
    CREATE TABLE [Search].[Selection](
        [SelectionId] [uniqueidentifier] NOT NULL,
        [SearchObjectId] [uniqueidentifier] NOT NULL,
        [ObjectFieldName] [nvarchar](128) NOT NULL,
        [Function] [nvarchar](64) NULL,
        [Aggregate] [nvarchar](64) NULL,

        [DateCreated] [datetime] NOT NULL,
        [CreatedBy] [uniqueidentifier] NOT NULL,
        [DateModified] [datetime] NOT NULL,
        [ModifiedBy] [uniqueidentifier] NOT NULL,
        [IsActive] [bit] NOT NULL,
        CONSTRAINT [PK_Selection] PRIMARY KEY CLUSTERED 
        (
            [SelectionId] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
GO


IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Selection_SearchObject' AND Type = 'F') BEGIN
    ALTER TABLE [Search].[Selection]
        ADD CONSTRAINT FK_Selection_SearchObject
            FOREIGN KEY (SearchObjectId) REFERENCES [Search].[SearchObject](SearchObjectId);
END