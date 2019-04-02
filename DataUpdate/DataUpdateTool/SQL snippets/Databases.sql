SELECT
	[database_name]
FROM
	[master].[sys].[databases]
WHERE
	[name] NOT IN ('master','tempdb','model','msdb')