IF OBJECT_ID('System.GetCompiledPage') IS NOT NULL BEGIN
	DROP PROCEDURE [System].[GetCompiledPage]
END
GO

CREATE PROCEDURE [System].[GetCompiledPage]
	@PageName NVARCHAR(256)
AS
BEGIN
	SELECT t1.CompiledPageId, t1.DateCreated, t1.CreatedBy, t1.PageId, t1.CompiledPage
	FROM 
		System.CompiledPage t1
		JOIN
		System.Page t2
		ON t1.PageId = t2.PageId
	WHERE t2.PageName = @PageName
END
GO

IF OBJECT_ID('System.SaveCompiledPage') IS NOT NULL BEGIN
	DROP PROCEDURE [System].[SaveCompiledPage]
END
GO

CREATE PROCEDURE [System].[SaveCompiledPage]
	@PageName NVARCHAR(256),
    @UserId UNIQUEIDENTIFIER,
    @CompiledPage NVARCHAR(MAX)
AS
BEGIN
    DECLARE @PageId UNIQUEIDENTIFIER = (SELECT PageId FROM System.Page WHERE PageName = @PageName);
    
    IF EXISTS(SELECT * FROM System.CompiledPage WHERE PageId = @PageId) BEGIN
        DELETE 
        FROM System.CompiledPage
        WHERE PageId = @PageId;
    END

    INSERT INTO System.CompiledPage(CompiledPageId, DateCreated, CreatedBy, PageId, CompiledPage)
    VALUES (NEWID(), GETUTCDATE(), @UserId, @PageId, @CompiledPage);
END
GO