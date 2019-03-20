IF OBJECT_ID('System.LookupModule') IS NOT NULL 
    DROP FUNCTION [System].[LookupModule]
GO
CREATE FUNCTION [System].[LookupModule](@Name nvarchar(128))
RETURNS UNIQUEIDENTIFIER
AS
BEGIN
    RETURN (
        SELECT ModuleId 
        FROM [System].[Module]
        WHERE Name = @Name
    )
END;