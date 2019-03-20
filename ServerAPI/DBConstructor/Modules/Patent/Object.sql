IF NOT EXISTS(SELECT * FROM [System].[Object] WHERE Name = 'Family' AND ModuleId = [System].[LookupModule]('Patent')) BEGIN
    INSERT INTO [System].[Object] (
        [ObjectId],
        [Name],
        [Description],
        [ModuleId],
        [ParentObjectId],
        [MainObjectId],
        [IsBaseObject],
        [DateCreated], [CreatedBy],
        [DateModified], [ModifiedBy],
        [IsActive]
    ) VALUES (
        NEWID(),
        'Family',
        'Patent Family Object.',
        
        [System].[LookupModule]('Patent'),
        NULL,
        NULL,
        1,

        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000',
        GETUTCDATE(), '00000000-0000-0000-0000-000000000000',
        1
    )
END

IF NOT EXISTS(SELECT * FROM [System].[Object] WHERE Name = 'Application' AND ModuleId = [System].[LookupModule]('Patent')) BEGIN
    INSERT INTO [System].[Object] (
        [ObjectId],
        [Name],
        [Description],
        [ModuleId],
        [ParentObjectId],
        [MainObjectId],
        [IsBaseObject],
        [DateCreated], [CreatedBy],
        [DateModified], [ModifiedBy],
        [IsActive]
    ) VALUES (
        NEWID(),
        'Application',
        'Patent Application Object.',
        
        [System].[LookupModule]('Patent'),
        NULL,
        NULL,
        1,

        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000',
        GETUTCDATE(), '00000000-0000-0000-0000-000000000000',
        1
    )
END

IF NOT EXISTS(SELECT * FROM [System].[Object] WHERE Name = 'Claim' AND ModuleId = [System].[LookupModule]('Patent')) BEGIN
    INSERT INTO [System].[Object] (
        [ObjectId],
        [Name],
        [Description],
        [ModuleId],
        [ParentObjectId],
        [MainObjectId],
        [IsBaseObject],
        [DateCreated], [CreatedBy],
        [DateModified], [ModifiedBy],
        [IsActive]
    ) VALUES (
        NEWID(),
        'Claim',
        'Patent Claim Object.',
        
        [System].[LookupModule]('Patent'),
        [System].[LookupObject]('Patent', 'Application'),
        NULL,
        1,

        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000',
        GETUTCDATE(), '00000000-0000-0000-0000-000000000000',
        1
    )
END

IF NOT EXISTS(SELECT * FROM [System].[Object] WHERE Name = 'Annuity' AND ModuleId = [System].[LookupModule]('Patent')) BEGIN
    INSERT INTO [System].[Object] (
        [ObjectId],
        [Name],
        [Description],
        [ModuleId],
        [ParentObjectId],
        [MainObjectId],
        [IsBaseObject],
        [DateCreated], [CreatedBy],
        [DateModified], [ModifiedBy],
        [IsActive]
    ) VALUES (
        NEWID(),
        'Annuity',
        'Patent Annuity Object.',
        
        [System].[LookupModule]('Patent'),
        [System].[LookupObject]('Patent', 'Application'),
        NULL,
        1,

        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000',
        GETUTCDATE(), '00000000-0000-0000-0000-000000000000',
        1
    )
END