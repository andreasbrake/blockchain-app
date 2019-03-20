IF OBJECT_ID('System.ObjectField') IS NULL
    CREATE TABLE [System].[ObjectField](
        [ObjectFieldId] [uniqueidentifier] NOT NULL,
        [Name] [nvarchar](128) NOT NULL,
        [Description] [nvarchar](512) NOT NULL,
        [ObjectId] [uniqueidentifier] NOT NULL,
        [DataType] [nvarchar](64) NOT NULL,

        [IsBaseIndexed] [bit] NOT NULL,

        [DateCreated] [datetime] NOT NULL,
        [CreatedBy] [uniqueidentifier] NOT NULL,
        [DateModified] [datetime] NOT NULL,
        [ModifiedBy] [uniqueidentifier] NOT NULL,
        [IsActive] [bit] NOT NULL,
        CONSTRAINT [UC_ObjectField_Name] UNIQUE (Name, ObjectId),
        CONSTRAINT [PK_ObjectField] PRIMARY KEY CLUSTERED 
        (
            [ObjectFieldId] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
GO

IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_ObjectField_Object' AND Type = 'F') BEGIN
    ALTER TABLE [System].[ObjectField]
        ADD CONSTRAINT FK_ObjectField_Object
            FOREIGN KEY (ObjectId) REFERENCES [System].[Object](ObjectId);
END