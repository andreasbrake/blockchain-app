IF OBJECT_ID('System.CleanTempSearches') IS NOT NULL BEGIN
    DROP PROCEDURE [System].[CleanTempSearches]
END
GO

CREATE PROCEDURE [System].[CleanTempSearches]
AS
BEGIN
	-- CLEAN SELECTION
	DELETE t1
	FROM 
		Search.Selection t1
		JOIN
		Search.SearchObject t2
		ON t1.SearchObjectId = t2.SearchObjectId
	WHERE t2.Name LIKE 'Unnamed-%' AND t2.Description = 'Unnamed search' AND DATEDIFF(HOUR, t2.DateModified, GETUTCDATE()) >= 24


	-- CLEAN OBJECT
	DELETE 
	FROM Search.SearchObject
	WHERE Name LIKE 'Unnamed-%' AND Description = 'Unnamed search' AND DATEDIFF(HOUR, DateModified, GETUTCDATE()) >= 24
END
GO

EXEC [System].[CleanTempSearches]