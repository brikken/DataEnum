﻿SELECT
	 OBJECT_SCHEMA_NAME([object_id]) [schema_name]
	,OBJECT_NAME([object_id]) [table_name]
	,[name] [index_name]
FROM
	[Misc].[sys].[indexes]