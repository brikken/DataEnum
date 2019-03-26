SELECT
	 [name]
	,[database_id]
FROM
	[master].[sys].[databases]
WHERE
	[name] NOT IN ('master','tempdb','model','msdb')