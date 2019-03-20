IF OBJECT_ID('System.RenderPage') IS NOT NULL BEGIN
	DROP PROCEDURE [System].[RenderPage]
END
GO

CREATE PROCEDURE [System].[RenderPage]
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

--EXEC [System].[RenderPage] 'TestPage'