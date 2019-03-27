SELECT
	 [name]
	,OBJECT_SCHEMA_NAME([object_id]) [schema_name]
	,OBJECT_NAME([object_id]) [primary_table_name]
	,OBJECT_SCHEMA_NAME([parent_object_id]) [foreign_table_name]
	,OBJECT_NAME([parent_object_id]) [foreign_table_name]
FROM
	[Misc].[sys].[foreign_keys]