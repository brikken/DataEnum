SELECT
	 OBJECT_SCHEMA_NAME(t.[object_id]) [schema_name]
	,t.[table_name]
	,COALESCE(c.is_identity, 0) [has_identity]
FROM
	[Misc].[sys].[tables] t
	LEFT JOIN [Misc].[sys].[columns] c ON
		c.object_id = t.object_id AND
		c.is_identity = 1