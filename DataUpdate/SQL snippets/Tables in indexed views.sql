select 
	OBJECT_SCHEMA_NAME(sed.referenced_id) as [table_schema],
	OBJECT_NAME(sed.referenced_id) as [table_name],
    OBJECT_SCHEMA_NAME(i.object_id) as [view_schema],
    OBJECT_NAME(i.object_id) as [view_name],
	i.[name] [index_name]
from
	sys.indexes i
	inner join sys.views v on
		v.object_id = i.object_id
	inner join sys.sql_expression_dependencies sed on
		sed.referencing_id = i.object_id
where
	sed.referenced_minor_id = 0 --the table itself, not the used columns
