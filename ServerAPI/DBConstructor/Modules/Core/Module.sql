IF NOT EXISTS(SELECT * FROM [System].[Module] WHERE Name = 'Core') BEGIN
    INSERT INTO [System].[Module] (
        [ModuleId],
        [Name],
        [Description],
        [DateCreated], [CreatedBy],
        [DateModified], [ModifiedBy],
        [IsActive]
    ) VALUES (
        NEWID(),
        'Core',
        'Core Module for all general application relevant objects.',
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000',
        GETUTCDATE(), '00000000-0000-0000-0000-000000000000',
        1
    )
END