IF OBJECT_ID('System.CreateWidget') IS NOT NULL BEGIN
	DROP PROCEDURE [System].[CreateWidget]
END
GO
CREATE PROCEDURE [System].[CreateWidget]
	@UserId UNIQUEIDENTIFIER,
	@WidgetName NVARCHAR(256),
	@WidgetTemplate NVARCHAR(MAX),
	@ModuleId UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @NewId UNIQUEIDENTIFIER = (SELECT WidgetId FROM System.Widget WHERE WidgetName = @WidgetName)
	IF @NewId IS NOT NULL BEGIN
		EXEC [System].[UpdateWidget]
			@Userid = @UserId,
			@WidgetName = @WidgetName,
			@WidgetTemplate = @WidgetTemplate
	END
	ELSE BEGIN
		SET @NewId = NEWID();
		INSERT INTO Widget(
			WidgetId, 
			CreatedBy, 
			DateCreated, 
			ModifiedBy, 
			DateModified, 
			WidgetName, 
			WidgetTemplate, 
			ModuleId
		)
		VALUES (
			@NewId, 
			@UserId, 
			GETUTCDATE(), 
			@UserId, 
			GETUTCDATE(), 
			@WidgetName, 
			@WidgetTemplate, 
			@ModuleId
		)
	END

	SELECT WidgetId, CreatedBy, DateCreated, ModifiedBy, DateModified, WidgetName, WidgetTemplate, ModuleId
	FROM System.Widget
	WHERE WidgetId = @NewId;
END
GO


IF OBJECT_ID('System.UpdateWidget') IS NOT NULL BEGIN
	DROP PROCEDURE [System].[UpdateWidget]
END
GO
CREATE PROCEDURE [System].[UpdateWidget]
	@UserId UNIQUEIDENTIFIER,
	@WidgetName NVARCHAR(256),
	@WidgetTemplate NVARCHAR(MAX)
AS
BEGIN
	UPDATE Widget
		SET WidgetName = @WidgetName,
			WidgetTemplate = @WidgetTemplate,
			ModifiedBy = @UserId,
			DateModified = GETUTCDATE()
	WHERE WidgetName = @WidgetName
END
GO

IF OBJECT_ID('System.GetWidget') IS NOT NULL BEGIN
	DROP PROCEDURE [System].[GetWidget]
END
GO
CREATE PROCEDURE [System].[GetWidget]
	@WidgetName NVARCHAR(256)
AS
BEGIN
	SELECT WidgetId, DateCreated, CreatedBy, DateModified, ModifiedBy, WidgetName, WidgetTemplate, ModuleId
	FROM Widget
	WHERE WidgetName = @WidgetName
END
GO

IF OBJECT_ID('System.GetWidgetsOnPage') IS NOT NULL BEGIN
	DROP PROCEDURE [System].[GetWidgetsOnPage]
END
GO
CREATE PROCEDURE [System].[GetWidgetsOnPage]
	@PageName NVARCHAR(256)
AS
BEGIN
	SELECT t1.WidgetId, t1.DateCreated, t1.CreatedBy, t1.DateModified, t1.ModifiedBy, t1.WidgetName, t1.WidgetTemplate, t1.ModuleId
	FROM 
		Widget t1
		JOIN
		PageWidget t2
		ON t1.WidgetId = t2.WidgetId
		JOIN
		Page t3
		ON t2.PageId = t3.PageId
	WHERE t3.PageName = @PageName
END
GO