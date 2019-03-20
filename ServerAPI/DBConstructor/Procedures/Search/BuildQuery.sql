IF OBJECT_ID('System.BuildQuery') IS NOT NULL BEGIN
	DROP PROCEDURE [System].[BuildQuery]
END
GO

CREATE PROCEDURE [System].[BuildQuery]
	@FieldList NVARCHAR(MAX)
AS
BEGIN
	SET QUOTED_IDENTIFIER ON;

	DECLARE @Fields AS TABLE ([FieldId] NVARCHAR(128), [Aggregate] NVARCHAR(128));
	DECLARE @FieldSQL NVARCHAR(MAX) = 'SELECT [FieldId], [Aggregate] FROM (VALUES ' + @FieldList + ')X([FieldId],[Aggregate])'

	INSERT INTO @Fields
		EXEC(@FieldSQL);

	DECLARE @fromQuery NVARCHAR(MAX) = 'FROM [dbo].[' + (
		SELECT TOP 1 t3.Name
		FROM 
			@Fields t1
			JOIN
			System.ObjectField t2
			ON t1.FieldId = t2.Name
			JOIN
			System.Object t3
			ON t2.ObjectId = t3.ObjectId
	) + ']'

	DECLARE @baseSelect NVARCHAR(MAX) = 'SELECT ' + 
		STUFF((
			SELECT 
				',' + 
				ISNULL(
					[Aggregate] + '([' + [FieldId] + '])',
					'[' + [FieldId] + ']'
				) + '[' + CASE 
					WHEN [Aggregate] = '' 
					THEN '' ELSE [Aggregate] + ' of ' 
				END + [FieldId] + ']'
			FROM @Fields
			FOR XML PATH, TYPE
		).value('.[1]','NVARCHAR(MAX)'),1,1,'') + ' 
		' + @fromQuery + '
		' + ISNULL('GROUP BY ' + STUFF((
			SELECT ',[' + [FieldId] + ']'
			FROM @Fields
			WHERE Aggregate = ''
			FOR XML PATH, TYPE
		).value('.[1]','NVARCHAR(MAX)'),1,1,''), '')

	DECLARE @searchQuery NVARCHAR(MAX) = @baseSelect + '
		ORDER BY ' + 
		ISNULL((
			SELECT '[' + [FieldId] + ']'
			FROM @Fields
			WHERE [FieldId] = 'DateModified' AND [Aggregate] = ''
		),(
			SELECT TOP 1 CASE
				WHEN [Aggregate] = '' THEN '[' + [FieldId] + ']'
				ELSE '[' + [Aggregate] + ' of ' + [FieldId] + ']'
			END
			FROM @Fields
		)) + 
		'
		OFFSET @StartIndex ROWS
		FETCH NEXT (@EndIndex - @StartIndex + 1) ROWS ONLY'

	DECLARE @countQuery NVARCHAR(MAX) = 'SELECT @count = COUNT(*) FROM (' + @baseSelect + ') t1'
	
	SELECT @countQuery AS [CountQuery], @searchQuery AS [SearchQuery]

	SELECT 
		t1.FieldId as [FieldName], 
		CASE 
			WHEN t1.Aggregate = '' 
			THEN '' ELSE t1.Aggregate + ' of ' 
		END + ISNULL(t2.Name, t3.DisplayName) as [DisplayName], 
		ISNULL(t2.Name, t3.DataType) as [DataType]
	FROM
		@Fields t1
		LEFT JOIN
		System.ObjectField t2
		ON t1.FieldId = t2.Name
		LEFT JOIN
		(
			SELECT *
			FROM (VALUES
				('DateCreated', 'Date Created', 'datetime'),
				('DateModified', 'Date Modified', 'datetime'),
				('CreatedBy', 'Created By', 'uniqueidentifier'),
				('ModifiedBy', 'Modified By', 'uniqueidentifier')
			)X([SystemName],[Displayname],[DataType])
		) t3
		ON t1.FieldId = t3.Displayname
END
GO

--EXEC System.[BuildQuery] '(''SubmittalNumber'',''Count'')' test
EXEC System.BuildQuery @FieldList='(''Paralegal'',''''),(''Case Number'',''Count'')'