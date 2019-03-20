IF OBJECT_ID('System.LookupObject') IS NOT NULL 
    DROP FUNCTION [System].[LookupObject]
GO
CREATE FUNCTION [System].[LookupObject](@ModuleName nvarchar(128), @Name nvarchar(128))
RETURNS UNIQUEIDENTIFIER
AS
BEGIN
    RETURN (
        SELECT ObjectId 
        FROM [System].[Object]
        WHERE Name = @Name AND ModuleId = (
            SELECT ModuleId
            FROM [System].[Module]
            WHERE Name = @ModuleName
        )
    )
END;