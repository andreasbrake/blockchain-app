IF OBJECT_ID('System.CreatePage') IS NOT NULL
	DROP PROCEDURE [System].[CreatePage]
GO

CREATE PROCEDURE [System].[CreatePage]
	@UserId UNIQUEIDENTIFIER,
	@PageName NVARCHAR(256),
	@PageTemplate NVARCHAR(MAX),
	@ModuleId UNIQUEIDENTIFIER
AS
BEGIN
	INSERT INTO Page(
		PageId, 
		CreatedBy, 
		DateCreated, 
		ModifiedBy, 
		DateModified, 
		PageName, 
		PageTemplate, 
		ModuleId
	)
	VALUES (
		NEWID(), 
		@UserId, 
		GETUTCDATE(), 
		@UserId, 
		GETUTCDATE(), 
		@PageName, 
		@PageTemplate, 
		@ModuleId
	)
END
GO

IF OBJECT_ID('System.UpdatePage') IS NOT NULL
	DROP PROCEDURE [System].[UpdatePage]
GO

CREATE PROCEDURE [System].[UpdatePage]
	@UserId UNIQUEIDENTIFIER,
	@PageName NVARCHAR(256),
	@PageTemplate NVARCHAR(MAX),
	@WidgetIds NVARCHAR(MAX)
AS
BEGIN
	DECLARE @PageId UNIQUEIDENTIFIER = (SELECT PageId FROM System.Page WHERE PageName = @PageName);
	DECLARE @WidgetIdTable AS TABLE ([WidgetName] NVARCHAR(128));
	IF @WidgetIds <> '' BEGIN
		DECLARE @WidgetIdSQL NVARCHAR(MAX) = 'SELECT distinct [WidgetId] FROM (VALUES ' + @WidgetIds + ')X([WidgetId])';
		INSERT INTO @WidgetIdTable 
			EXEC(@WidgetIdSQL);
	END

	DELETE t1
	FROM
		System.PageWidget t1
		LEFT JOIN
		System.Widget t2
		ON t1.WidgetId = t2.WidgetId
		LEFT JOIN
		@WidgetIdTable t3
		ON t2.WidgetName = t3.WidgetName
	WHERE t1.PageId = @PageId AND t3.WidgetName IS NULL


	INSERT INTO System.PageWidget(PageWidgetId, DateCreated, CreatedBy, PageId, WidgetId)
	SELECT NEWID(), GETUTCDATE(), @UserId, @PageId, t2.WidgetId
	FROM
		@WidgetIdTable t1
		JOIN
		System.Widget t2
		ON t1.WidgetName = t2.WidgetName
		LEFT JOIN
		System.PageWidget t3
		ON t2.WidgetId = t3.WidgetId
	WHERE t3.WidgetId IS NULL

	UPDATE Page
		SET PageName = @PageName,
			PageTemplate = @PageTemplate,
			ModifiedBy = @UserId,
			DateModified = GETUTCDATE()
	WHERE PageName = @PageName
END
GO

IF OBJECT_ID('System.GetPage') IS NOT NULL
	DROP PROCEDURE [System].[GetPage]
GO

CREATE PROCEDURE [System].[GetPage]
	@PageName NVARCHAR(256)
AS
BEGIN
	SELECT PageId, DateCreated, CreatedBy, DateModified, ModifiedBy, PageName, PageTemplate, ModuleId
	FROM Page
	WHERE PageName = @PageName
END
GO

IF OBJECT_ID('System.GetPageAndWidgets') IS NOT NULL BEGIN
	DROP PROCEDURE [System].[GetPageAndWidgets]
END
GO

CREATE PROCEDURE [System].[GetPageAndWidgets]
	@PageName NVARCHAR(256)
AS
BEGIN
	SELECT PageId, DateCreated, CreatedBy, DateModified, ModifiedBy, PageName, PageTemplate, ModuleId
	FROM System.Page
	WHERE PageName = @PageName

	
	SELECT t3.WidgetId, t3.DateCreated, t3.CreatedBy, t3.DateModified, t3.ModifiedBy, t3.WidgetName, t3.WidgetTemplate, t3.ModuleId
	FROM 
		System.Page t1
		JOIN
		System.PageWidget t2
		ON t1.PageId = t2.PageId
		JOIN
		System.Widget t3
		ON t2.WidgetId = t3.WidgetId
	WHERE PageName = @PageName
END
GO