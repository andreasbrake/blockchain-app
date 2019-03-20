IF NOT EXISTS(SELECT * FROM [System].[Object] WHERE Name = 'Jurisdiction' AND ModuleId = [System].[LookupModule]('Core')) BEGIN
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
        'Jurisdiction',
        'List of application wide jurisdictions.',

        [System].[LookupModule]('Core'),
        NULL,
        NULL,
        0,

        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000',
        GETUTCDATE(), '00000000-0000-0000-0000-000000000000',
        1
    )
END


IF NOT EXISTS(SELECT * FROM [System].[Object] WHERE Name = 'Task' AND ModuleId = [System].[LookupModule]('Core')) BEGIN
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
        'Task',
        'Application wide task list.',
        
        [System].[LookupModule]('Core'),
        NULL,
        NULL,
        0,

        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000',
        GETUTCDATE(), '00000000-0000-0000-0000-000000000000',
        1
    )
END
