---- CORE_JURISDICTION ----
IF NOT EXISTS(SELECT * FROM [System].[ObjectField] WHERE Name = 'Name' AND ObjectId = [System].[LookupObject]('Core', 'Jurisdiction')) BEGIN
    INSERT INTO [System].[ObjectField] (
        [ObjectFieldId], [ObjectId], 
        [Name], [Description], [DataType], [IsBaseIndexed],
        [DateCreated], [CreatedBy], [DateModified], [ModifiedBy], [IsActive]
    ) VALUES (
        NEWID(), [System].[LookupObject]('Core', 'Jurisdiction'),
        'Name', 'Jurisdiction Name.', '[nvarchar](128)', 0,
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000', GETUTCDATE(), '00000000-0000-0000-0000-000000000000', 1
    )
END

IF NOT EXISTS(SELECT * FROM [System].[ObjectField] WHERE Name = 'Description' AND ObjectId = [System].[LookupObject]('Core', 'Jurisdiction')) BEGIN
    INSERT INTO [System].[ObjectField] (
        [ObjectFieldId], [ObjectId], 
        [Name], [Description], [DataType], [IsBaseIndexed],
        [DateCreated], [CreatedBy], [DateModified], [ModifiedBy], [IsActive]
    ) VALUES (
        NEWID(), [System].[LookupObject]('Core', 'Jurisdiction'),
        'Description', 'Jurisdiction Description.', '[nvarchar](512)', 0,
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000', GETUTCDATE(), '00000000-0000-0000-0000-000000000000', 1
    )
END

---- CORE_TASK ----
IF NOT EXISTS(SELECT * FROM [System].[ObjectField] WHERE Name = 'Name' AND ObjectId = [System].[LookupObject]('Core', 'Task')) BEGIN
    INSERT INTO [System].[ObjectField] (
        [ObjectFieldId], [ObjectId], 
        [Name], [Description], [DataType], [IsBaseIndexed],
        [DateCreated], [CreatedBy], [DateModified], [ModifiedBy], [IsActive]
    ) VALUES (
        NEWID(), [System].[LookupObject]('Core', 'Task'),
        'Name', 'Task Name.', '[nvarchar](128)', 0,
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000', GETUTCDATE(), '00000000-0000-0000-0000-000000000000', 1
    )
END

IF NOT EXISTS(SELECT * FROM [System].[ObjectField] WHERE Name = 'Description' AND ObjectId = [System].[LookupObject]('Core', 'Task')) BEGIN
    INSERT INTO [System].[ObjectField] (
        [ObjectFieldId], [ObjectId], 
        [Name], [Description], [DataType], [IsBaseIndexed],
        [DateCreated], [CreatedBy], [DateModified], [ModifiedBy], [IsActive]
    ) VALUES (
        NEWID(), [System].[LookupObject]('Core', 'Task'),
        'Description', 'Task Description.', '[nvarchar](512)', 0,
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000', GETUTCDATE(), '00000000-0000-0000-0000-000000000000', 1
    )
END

IF NOT EXISTS(SELECT * FROM [System].[ObjectField] WHERE Name = 'DueDate' AND ObjectId = [System].[LookupObject]('Core', 'Task')) BEGIN
    INSERT INTO [System].[ObjectField] (
        [ObjectFieldId], [ObjectId], 
        [Name], [Description], [DataType], [IsBaseIndexed],
        [DateCreated], [CreatedBy], [DateModified], [ModifiedBy], [IsActive]
    ) VALUES (
        NEWID(), [System].[LookupObject]('Core', 'Task'),
        'DueDate', 'Task Due Date.', '[datetime]', 0,
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000', GETUTCDATE(), '00000000-0000-0000-0000-000000000000', 1
    )
END

IF NOT EXISTS(SELECT * FROM [System].[ObjectField] WHERE Name = 'ReminderDate' AND ObjectId = [System].[LookupObject]('Core', 'Task')) BEGIN
    INSERT INTO [System].[ObjectField] (
        [ObjectFieldId], [ObjectId], 
        [Name], [Description], [DataType], [IsBaseIndexed],
        [DateCreated], [CreatedBy], [DateModified], [ModifiedBy], [IsActive]
    ) VALUES (
        NEWID(), [System].[LookupObject]('Core', 'Task'),
        'ReminderDate', 'Task Reminder Date.', '[datetime]', 0,
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000', GETUTCDATE(), '00000000-0000-0000-0000-000000000000', 1
    )
END

IF NOT EXISTS(SELECT * FROM [System].[ObjectField] WHERE Name = 'ReminderSchedule' AND ObjectId = [System].[LookupObject]('Core', 'Task')) BEGIN
    INSERT INTO [System].[ObjectField] (
        [ObjectFieldId], [ObjectId], 
        [Name], [Description], [DataType], [IsBaseIndexed],
        [DateCreated], [CreatedBy], [DateModified], [ModifiedBy], [IsActive]
    ) VALUES (
        NEWID(), [System].[LookupObject]('Core', 'Task'),
        'ReminderSchedule', 'Task Reminder Calculation Scheulde.', '[nvarchar](512)', 0,
        '2018-02-27 03:37:53.543', '00000000-0000-0000-0000-000000000000', GETUTCDATE(), '00000000-0000-0000-0000-000000000000', 1
    )
END