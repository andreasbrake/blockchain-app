IF OBJECT_ID('System.Module') IS NULL
    CREATE TABLE [System].[Module](
        [ModuleId] [uniqueidentifier] NOT NULL,
        [Name] [nvarchar](32) NOT NULL,
        [Description] [nvarchar](512) NULL,
        
        [DateCreated] [datetime] NOT NULL,
        [CreatedBy] [uniqueidentifier] NOT NULL,
        [DateModified] [datetime] NOT NULL,
        [ModifiedBy] [uniqueidentifier] NOT NULL,
        [IsActive] [bit] NOT NULL,
        CONSTRAINT [UC_Module_Name] UNIQUE (Name),
        CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED 
        (
            [ModuleId] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
GO

