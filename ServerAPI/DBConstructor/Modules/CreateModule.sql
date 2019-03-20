IF OBJECT_ID('System.LoadModule') IS NOT NULL
	DROP PROCEDURE [System].[LoadModule]
GO
CREATE PROCEDURE [System].[LoadModule] 
    @ModuleName NVARCHAR(128)
AS
BEGIN
	DECLARE @TableCreation AS TABLE([Script] NVARCHAR(MAX))
	INSERT INTO @TableCreation
		SELECT 
			'IF OBJECT_ID(''dbo.' + t2.Name + '_' + t1.Name + ''') IS NULL BEGIN ' + 
			'CREATE TABLE [dbo].[' + t2.Name + '_' + t1.Name + '] (' + 
			'[' + t1.Name + 'Id] UNIQUEIDENTIFIER PRIMARY KEY,' +
			(
				CASE 
					WHEN t1.ParentObjectId IS NOT NULL
					THEN '[' + (
						SELECT tx.Name
						FROM System.Object tx
						WHERE tx.ObjectId = t1.ParentObjectId
					) + 'Id] UNIQUEIDENTIFIER,'
				ELSE
					''
				END
			) +
			'[DateCreated] DATETIME NOT NULL,' +
			'[CreatedBy] UNIQUEIDENTIFIER NOT NULL,' +
			'[DateModified] DATETIME NOT NULL,' +
			'[ModifiedBy] UNIQUEIDENTIFIER NOT NULL,' +
			ISNULL((
				SELECT '[' + f.Name + '] ' + f.DataType + ','
				FROM System.ObjectField f
				WHERE f.ObjectId = t1.ObjectId
				ORDER BY f.Name
				FOR XML PATH, TYPE
			).value('.[1]','NVARCHAR(MAX)'), '') +
			')' + 'END ' +
			ISNULL(' ELSE BEGIN ' +
			(
				SELECT 
					'IF NOT EXISTS(SELECT * FROM Sys.All_Columns WHERE Name = ''' + f.Name + ''' AND object_id = OBJECT_ID(''dbo.' + t2.Name + '_' + t1.Name + ''')) BEGIN ' +
					'ALTER TABLE [dbo].[' + t2.Name + '_' + t1.Name + '] ADD [' + f.Name + '] ' + f.DataType + 
					' END '
				FROM System.ObjectField f
				WHERE f.ObjectId = t1.ObjectId
				ORDER BY f.Name
				FOR XML PATH, TYPE
			).value('.[1]','NVARCHAR(MAX)') +
			' END ', '')
		FROM 
			System.Object t1
			JOIN
			System.Module t2
			ON t1.ModuleId = t2.ModuleId
			LEFT JOIN
			System.RelationshipObject t3
			ON t1.ObjectId = t3.RelationshipId
		WHERE t2.Name = @ModuleName AND t3.RelationshipId IS NULL

	DECLARE @combined NVARCHAR(MAX) = (
		SELECT [Script] + '
		'
		FROM @TableCreation
		FOR XML PATH, TYPE
	).value('.[1]','NVARCHAR(MAX)')

	SELECT *
	FROM @TableCreation

	EXEC(@combined)
END
GO

EXEC [System].[LoadModule] 'Patent'