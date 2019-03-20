IF OBJECT_ID('Search.SearchObject') IS NULL
    CREATE TABLE [Search].[SearchObject](
        [SearchObjectId] [uniqueidentifier] NOT NULL,
        [ ] [nvarchar](256) NOT NULL,
        [Description] [nvarchar](512) NULL,
        [ModuleName] [nvarchar](128) NOT NULL,
        [ObjectName] [nvarchar](128) NOT NULL,

        [CompiledQuery] [nvarchar](max) NULL,
        [CompiledCountQuery] [nvarchar](max) NULL,
        [CompiledFieldList] [nvarchar](max) NULL,
        [IsValid] [bit] NOT NULL,
        
        [DateCreated] [datetime] NOT NULL,
        [CreatedBy] [uniqueidentifier] NOT NULL,
        [DateModified] [datetime] NOT NULL,
        [ModifiedBy] [uniqueidentifier] NOT NULL,
        [IsActive] [bit] NOT NULL,
        CONSTRAINT [UC_SearchObject_Name] UNIQUE (Name),
        CONSTRAINT [PK_SearchObject] PRIMARY KEY CLUSTERED 
        (
            [SearchObjectId] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
GO

