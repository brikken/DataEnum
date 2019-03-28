SELECT
	 OBJECT_SCHEMA_NAME(c.[OBJECT_ID]) [schema_name]
	,OBJECT_NAME(c.[object_id]) [table_name]
	,c.[name] [column_name]
	,c.[column_id]
	,t.[is_assembly_type]
FROM
	[Misc].[sys].[columns] c
	INNER JOIN [Misc].[sys].[objects] o ON
		o.[object_id] = c.[object_id] AND
		o.[type] = 'U'
	INNER JOIN [Misc].[sys].[types] t ON
		t.[system_type_id] = c.[system_type_id] AND
		t.[user_type_id] = c.[user_type_id]