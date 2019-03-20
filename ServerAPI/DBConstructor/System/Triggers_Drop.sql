--- Object ---
    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Object_Module' AND Type = 'F') BEGIN
        ALTER TABLE [System].[Object]
            DROP CONSTRAINT FK_Object_Object_Module;
    END

    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Object_Object_Parent' AND Type = 'F') BEGIN
        ALTER TABLE [System].[Object]
            DROP CONSTRAINT FK_Object_Object_Parent;
    END

    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Object_Object_Main' AND Type = 'F') BEGIN
        ALTER TABLE [System].[Object]
            DROP CONSTRAINT FK_Object_Object_Main;
    END

--- Relationship Object -----
    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_RelationshipObject_Object' AND Type = 'F') BEGIN
        ALTER TABLE [System].[RelationshipObject]
            DROP CONSTRAINT FK_RelationshipObject_Object;
    END

    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_RelationshipObject_Object_Object1' AND Type = 'F') BEGIN
        ALTER TABLE [System].[RelationshipObject]
            DROP CONSTRAINT FK_RelationshipObject_Object_Object1;
    END

    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_RelationshipObject_Object_Object2' AND Type = 'F') BEGIN
       ALTER TABLE [System].[RelationshipObject]
            DROP CONSTRAINT FK_RelationshipObject_Object_Object2;
    END

--- Object Field -
    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_ObjectField_Object' AND Type = 'F') BEGIN
        ALTER TABLE [System].[ObjectField]
            DROP CONSTRAINT FK_ObjectField_Object;
    END

--- Object Field Widget ---
    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_ObjectFieldWidget_ObjectField' AND Type = 'F') BEGIN
        ALTER TABLE [System].[ObjectFieldWidget]
            DROP CONSTRAINT FK_ObjectFieldWidget_ObjectField;
    END


    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_ObjectFieldWidget_Widget' AND Type = 'F') BEGIN
        ALTER TABLE [System].[ObjectFieldWidget]
            DROP CONSTRAINT FK_ObjectFieldWidget_Widget;
    END

--- Page ---
    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Page_Module' AND Type = 'F') BEGIN
        ALTER TABLE [System].[Page]
            DROP CONSTRAINT FK_Page_Module;
    END

--- Page Widget ---
    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_PageWidget_Page' AND Type = 'F') BEGIN
        ALTER TABLE [System].[PageWidget]
            DROP CONSTRAINT FK_PageWidget_Page;
    END

    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_PageWidget_Widget' AND Type = 'F') BEGIN
        ALTER TABLE [System].[PageWidget]
            DROP CONSTRAINT FK_PageWidget_Widget;
    END

    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_PageWidget_Relationship' AND Type = 'F') BEGIN
        ALTER TABLE [System].[PageWidget]
            DROP CONSTRAINT FK_PageWidget_Relationship;
    END

--- Widget ---
    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Widget_Module' AND Type = 'F') BEGIN
        ALTER TABLE [System].[Widget]
            DROP CONSTRAINT FK_Widget_Module;
    END

--- Widget Tree ---
    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_WidgetTree_Widget' AND Type = 'F') BEGIN
        ALTER TABLE [System].[WidgetTree]
            DROP CONSTRAINT FK_WidgetTree_Widget;
    END

    IF EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_WidgetTree_ChildWidget' AND Type = 'F') BEGIN
        ALTER TABLE [System].[WidgetTree]
            DROP CONSTRAINT FK_WidgetTree_ChildWidget;
    END

