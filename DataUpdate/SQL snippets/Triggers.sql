-- Triggers must in same schema as table
SELECT
	 OBJECT_SCHEMA_NAME(t.[object_id]) [schema_name]
	,OBJECT_NAME([parent_id]) [table_name]
	,t.[name]
FROM
	[Misc].[sys].[triggers] t
	INNER JOIN [Misc].[sys].[objects] o ON
		o.[object_id] = t.parent_id AND
		o.[type] = 'U'