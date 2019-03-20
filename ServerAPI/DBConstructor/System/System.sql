IF NOT EXISTS(SELECT * FROM sys.schemas WHERE name = 'System')
    EXEC sp_executesql N'CREATE SCHEMA [System]'
GO