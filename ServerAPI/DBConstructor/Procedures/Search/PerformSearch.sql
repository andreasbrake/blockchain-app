IF OBJECT_ID('System.PerformSizeSearch') IS NOT NULL BEGIN
	DROP PROCEDURE [System].[PerformSizeSearch]
END
GO

CREATE PROCEDURE [System].[PerformSizeSearch]
	@CountQuery NVARCHAR(MAX)
AS
BEGIN
	DECLARE @count INT = -1;
	EXEC sp_executesql 
		@countQuery, 
		N'@count INT OUTPUT', 
		@count = @count OUTPUT

	SELECT @count AS [Count]
END
GO

IF OBJECT_ID('System.PerformSearch') IS NOT NULL BEGIN
	DROP PROCEDURE [System].[PerformSearch]
END
GO

CREATE PROCEDURE [System].[PerformSearch]
	@SearchQuery NVARCHAR(MAX),
	@StartIndex INT,
	@EndIndex INT
AS
BEGIN
	EXEC sp_executesql 
		@SearchQuery, 
		N'@StartIndex INT, @EndIndex INT', 
		@StartIndex = @StartIndex, @EndIndex = @EndIndex
END
GO
