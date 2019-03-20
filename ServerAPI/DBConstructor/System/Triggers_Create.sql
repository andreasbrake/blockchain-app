--- Object ---
    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Object_Module' AND Type = 'F') BEGIN
        ALTER TABLE [System].[Object]
            ADD CONSTRAINT FK_Object_Module
                FOREIGN KEY (ModuleId) REFERENCES [System].[Module](ModuleId);
    END

    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Object_Object_Parent' AND Type = 'F') BEGIN
        ALTER TABLE [System].[Object]
            ADD CONSTRAINT FK_Object_Object_Parent
                FOREIGN KEY (ParentObjectId) REFERENCES [System].[Object](ObjectId);
    END

    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Object_Object_Main' AND Type = 'F') BEGIN
        ALTER TABLE [System].[Object]
            ADD CONSTRAINT FK_Object_Object_Main
                FOREIGN KEY (MainObjectId) REFERENCES [System].[Object](ObjectId);
    END

--- Relationship Object ---
    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_RelationshipObject_Object' AND Type = 'F') BEGIN
        ALTER TABLE [System].[RelationshipObject]
            ADD CONSTRAINT FK_RelationshipObject_Object
                FOREIGN KEY (RelationshipId) REFERENCES [System].[Object](ObjectId);
    END

    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_RelationshipObject_Object_Object1' AND Type = 'F') BEGIN
        ALTER TABLE [System].[RelationshipObject]
            ADD CONSTRAINT FK_RelationshipObject_Object_Object1
                FOREIGN KEY (Object1Id) REFERENCES [System].[Object](ObjectId);
    END

    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_RelationshipObject_Object_Object2' AND Type = 'F') BEGIN
       ALTER TABLE [System].[RelationshipObject]
            ADD CONSTRAINT FK_RelationshipObject_Object_Object2
                FOREIGN KEY (Object2Id) REFERENCES [System].[Object](ObjectId);
    END

--- Object Field ---
    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_ObjectField_Object' AND Type = 'F') BEGIN
        ALTER TABLE [System].[ObjectField]
            ADD CONSTRAINT FK_ObjectField_Object
                FOREIGN KEY (ObjectId) REFERENCES [System].[Object](ObjectId);
    END

--- Object Field Widget ---
    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_ObjectFieldWidget_ObjectField' AND Type = 'F') BEGIN
        ALTER TABLE [System].[ObjectFieldWidget]
            ADD CONSTRAINT FK_ObjectFieldWidget_ObjectField
                FOREIGN KEY (ObjectFieldId) REFERENCES [System].[ObjectField](ObjectFieldId);
    END

    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_ObjectFieldWidget_Widget' AND Type = 'F') BEGIN
        ALTER TABLE [System].[ObjectFieldWidget]
            ADD CONSTRAINT FK_ObjectFieldWidget_Widget
                FOREIGN KEY (WidgetId) REFERENCES [System].[Widget](WidgetId);
    END

--- Page ---
    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Page_Module' AND Type = 'F') BEGIN
        ALTER TABLE [System].[Page]
            ADD CONSTRAINT FK_Page_Module
                FOREIGN KEY (ModuleId) REFERENCES [System].[Module](ModuleId);
    END

--- Page Widget ---
    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_PageWidget_Page' AND Type = 'F') BEGIN
        ALTER TABLE [System].[PageWidget]
            ADD CONSTRAINT FK_PageWidget_Page
                FOREIGN KEY (PageId) REFERENCES [System].[Page](PageId);
    END

    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_PageWidget_Widget' AND Type = 'F') BEGIN
        ALTER TABLE [System].[PageWidget]
            ADD CONSTRAINT FK_PageWidget_Widget
                FOREIGN KEY (WidgetId) REFERENCES [System].[Widget](WidgetId);
    END

    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_PageWidget_Relationship' AND Type = 'F') BEGIN
        ALTER TABLE [System].[PageWidget]
            ADD CONSTRAINT FK_PageWidget_Relationship
                FOREIGN KEY (RelationshipId) REFERENCES [System].[RelationshipObject](RelationshipId);
    END


--- Widget ---
    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_Widget_Module' AND Type = 'F') BEGIN
        ALTER TABLE [System].[Widget]
            ADD CONSTRAINT FK_Widget_Module
                FOREIGN KEY (ModuleId) REFERENCES [System].[Module](ModuleId);
    END

--- Widget Tree ---
    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_WidgetTree_Widget' AND Type = 'F') BEGIN
        ALTER TABLE [System].[WidgetTree]
            ADD CONSTRAINT FK_WidgetTree_Widget
                FOREIGN KEY (WidgetId) REFERENCES [System].[Widget](WidgetId);
    END

    IF NOT EXISTS(SELECT * FROM Sys.all_objects WHERE Name = 'FK_WidgetTree_ChildWidget' AND Type = 'F') BEGIN
        ALTER TABLE [System].[WidgetTree]
            ADD CONSTRAINT FK_WidgetTree_ChildWidget
                FOREIGN KEY (ChildWidgetId) REFERENCES [System].[Widget](WidgetId);
    END

