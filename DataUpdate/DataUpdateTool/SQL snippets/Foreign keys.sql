SELECT
	 OBJECT_SCHEMA_NAME([referenced_object_id]) [primary_schema_name]
	,OBJECT_NAME([referenced_object_id]) [primary_table_name]
	,OBJECT_SCHEMA_NAME([parent_object_id]) [foreign_schema_name]
	,OBJECT_NAME([parent_object_id]) [foreign_table_name]
	,[name]
FROM
	[Misc].[sys].[foreign_keys]