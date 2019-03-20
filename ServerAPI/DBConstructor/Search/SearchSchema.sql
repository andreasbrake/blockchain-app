IF NOT EXISTS(SELECT * FROM sys.schemas WHERE name = 'Search')
    EXEC sp_executesql N'CREATE SCHEMA [Search]'
GO