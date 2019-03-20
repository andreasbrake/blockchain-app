IF OBJECT_ID('System.GetObjectData') IS NOT NULL BEGIN
    DROP PROCEDURE [System].[GetObjectData]
END
GO
CREATE PROCEDURE [System].[GetObjectData]
    @ObjectId NVARCHAR(256)
AS
BEGIN
    SELECT *
    FROM dbo.Submittal
    WHERE ObjectId = @ObjectId
END
GO