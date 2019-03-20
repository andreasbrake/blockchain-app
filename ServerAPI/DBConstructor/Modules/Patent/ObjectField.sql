---- Patent Application ----
IF NOT EXISTS(SELECT * FROM [System].[ObjectField] WHERE Name = 'Name' AND ObjectId = [System].[LookupObject]('Patent', 'Application')) BEGIN
    INSERT INTO [System].[ObjectField] (
        [ObjectFieldId], [ObjectId], 
        [Name], [Description], [DataType], [IsBaseIndexed],
        [DateCreated], [CreatedBy], [DateModified], [ModifiedBy], [IsActive]
    ) VALUES (
        NEWID(), [System].[LookupObject]('Patent', 'Application'),
        'Name', 'Patent Applciation Internal Id/Name.', '[nvarchar](128)', 0,
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000', GETUTCDATE(), '00000000-0000-0000-0000-000000000000', 1
    )
END

IF NOT EXISTS(SELECT * FROM [System].[ObjectField] WHERE Name = 'Title' AND ObjectId = [System].[LookupObject]('Patent', 'Application')) BEGIN
    INSERT INTO [System].[ObjectField] (
        [ObjectFieldId], [ObjectId], 
        [Name], [Description], [DataType], [IsBaseIndexed],
        [DateCreated], [CreatedBy], [DateModified], [ModifiedBy], [IsActive]
    ) VALUES (
        NEWID(), [System].[LookupObject]('Patent', 'Application'),
        'Title', 'Patent Applciation Title.', '[nvarchar](512)', 0,
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000', GETUTCDATE(), '00000000-0000-0000-0000-000000000000', 1
    )
END

IF NOT EXISTS(SELECT * FROM [System].[ObjectField] WHERE Name = 'ApplicationNumber' AND ObjectId = [System].[LookupObject]('Patent', 'Application')) BEGIN
    INSERT INTO [System].[ObjectField] (
        [ObjectFieldId], [ObjectId], 
        [Name], [Description], [DataType], [IsBaseIndexed],
        [DateCreated], [CreatedBy], [DateModified], [ModifiedBy], [IsActive]
    ) VALUES (
        NEWID(), [System].[LookupObject]('Patent', 'Application'),
        'ApplicationNumber', 'Patent Applciation Number.', '[nvarchar](128)', 0,
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000', GETUTCDATE(), '00000000-0000-0000-0000-000000000000', 1
    )
END

---- Patent Claim ----

IF NOT EXISTS(SELECT * FROM [System].[ObjectField] WHERE Name = 'Claim' AND ObjectId = [System].[LookupObject]('Patent', 'Claim')) BEGIN
    INSERT INTO [System].[ObjectField] (
        [ObjectFieldId], [ObjectId], 
        [Name], [Description], [DataType], [IsBaseIndexed],
        [DateCreated], [CreatedBy], [DateModified], [ModifiedBy], [IsActive]
    ) VALUES (
        NEWID(), [System].[LookupObject]('Patent', 'Claim'),
        'Claim', 'Patent Claim Details.', '[nvarchar](max)', 0,
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000', GETUTCDATE(), '00000000-0000-0000-0000-000000000000', 1
    )
END

IF NOT EXISTS(SELECT * FROM [System].[ObjectField] WHERE Name = 'Sequence' AND ObjectId = [System].[LookupObject]('Patent', 'Claim')) BEGIN
    INSERT INTO [System].[ObjectField] (
        [ObjectFieldId], [ObjectId], 
        [Name], [Description], [DataType], [IsBaseIndexed],
        [DateCreated], [CreatedBy], [DateModified], [ModifiedBy], [IsActive]
    ) VALUES (
        NEWID(), [System].[LookupObject]('Patent', 'Claim'),
        'Sequence', 'Patent Claim Sequence.', '[nvarchar](max)', 0,
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000', GETUTCDATE(), '00000000-0000-0000-0000-000000000000', 1
    )
END