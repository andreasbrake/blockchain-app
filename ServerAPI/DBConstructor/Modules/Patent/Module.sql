IF NOT EXISTS(SELECT * FROM [System].[Module] WHERE Name = 'Patent') BEGIN
    INSERT INTO [System].[Module] (
        [ModuleId],
        [Name],
        [Description],
        [DateCreated], [CreatedBy],
        [DateModified], [ModifiedBy],
        [IsActive]
    ) VALUES (
        NEWID(),
        'Patent',
        'Module for patents, applications, and all related objects.',
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000',
        GETUTCDATE(), '00000000-0000-0000-0000-000000000000',
        1
    )
END