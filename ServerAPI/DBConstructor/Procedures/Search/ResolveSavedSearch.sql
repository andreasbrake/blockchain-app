IF OBJECT_ID('System.ResolveSavedSearch') IS NOT NULL BEGIN
	DROP PROCEDURE [System].[ResolveSavedSearch]
END
GO

CREATE PROCEDURE [System].[ResolveSavedSearch]
    @SearchId UNIQUEIDENTIFIER
AS
BEGIN
    IF @SearchId = '4209803C-988F-4B49-A933-1B44660FB49E' BEGIN
        SELECT 
            'SELECT @count = 10' AS [CountQuery], 
            'SELECT TOP 10 Jurisdiction, COUNT(*) as [Count] FROM MOCK_DATA GROUP BY Jurisdiction ORDER BY COUNT(*) DESC'  AS [SearchQuery]

        SELECT *
        FROM (VALUES
            ('Jurisdiction', 'Jurisdiction', 'string'),
            ('Count', 'Count', 'integer')
        )X([FieldName],[DisplayName],[DataType])
    END
    
    IF @SearchId = 'A938FD1B-07AD-4A8A-A136-9751105B3377' BEGIN
        SELECT 
            'SELECT @count = COUNT(*)
            FROM (
                SELECT Jurisdiction
                FROM MOCK_DATA
                WHERE Jurisdiction IN (''United States'', ''China'')
                GROUP BY YEAR([Filing Date]), Jurisdiction
            ) x ' AS [CountQuery], 
            'SELECT YEAR([Filing Date]) as [FilingYear], COUNT(*) as [Count], Jurisdiction
                FROM MOCK_DATA
                WHERE Jurisdiction IN (''United States'', ''China'')
                GROUP BY YEAR([Filing Date]), Jurisdiction
                ORDER BY YEAR([Filing Date]), Jurisdiction'  AS [SearchQuery]

        SELECT *
        FROM (VALUES
		    ('FilingYear', 'Filing Year', 'date'),
            ('Count', 'Count', 'integer'),
            ('Jurisdiction', 'Jurisdiction', 'string')
        )X([FieldName],[DisplayName],[DataType])
    END
END
GO