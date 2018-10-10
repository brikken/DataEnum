-- Token count: 31
CREATE TABLE [dbo].MyBiTempTable (
	id INT NOT NULL PRIMARY KEY,
	myNum DECIMAL(18,2) NULL,
	myText VARCHAR(200) NOT NULL
)
--DTWITH (BITEMPORAL (BTSCHEMA = [bitemp]))
;
GO

CREATE TABLE [bitemp].MyBiTempTable (
	id INT NOT NULL /*PRIMARY KEY*/,
	myNum DECIMAL(18,2) NULL,
	myText VARCHAR(200) NOT NULL,
	validFrom DATETIME2(7) NULL,
	validTo DATETIME2(7) NULL,
	transBegin DATETIME2(7) NOT NULL,
	transEnd DATETIME2(7) NULL
)
GO

CREATE TRIGGER [bitemp].MyBiTempTable_DELETE ON [bitemp].[MyBiTempTable]
INSTEAD OF DELETE
AS BEGIN
	-- Deletes are never allowed in the transaction log
	ROLLBACK;
	THROW 50000, 'Deletes are not allowed from a bitemporal table', 0;
END
GO

CREATE TRIGGER [bitemp].MyBiTempTable_DML ON [bitemp].[MyBiTempTable]
AFTER INSERT, UPDATE
AS BEGIN
	IF (ROWCOUNT_BIG() = 0) RETURN; -- https://docs.microsoft.com/en-us/sql/t-sql/statements/create-trigger-transact-sql

	-- Since
	--   start <= T < end
	-- these rows will never have any significance
	DELETE [bitemp].[MyBiTempTable] WHERE
		ISNULL(validFrom, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) = ISNULL(validTo, CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999'))
		OR
		ISNULL(transBegin, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) = ISNULL(transEnd, CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')) -- Template: watch out for NULLs

	-- Optimization ideas
	--   SELECT TOP 1 [...] to stop as soon as an error is found
	--   The overlaps are symmetric - can that be utilized?
	--   Pre-optimize index and execution strategy (hints)
	DECLARE @entries BIGINT, @overlaps BIGINT
	SELECT
		@entries = COUNT_BIG(*)
	FROM
		(SELECT DISTINCT id FROM inserted) i
		INNER JOIN [bitemp].[MyBiTempTable] t1 ON 1=1
			AND t1.id = i.id

	SELECT
		@overlaps = COUNT_BIG(*)
	FROM
		(SELECT DISTINCT id FROM inserted) i
		INNER JOIN [bitemp].[MyBiTempTable] t1 ON 1=1
			AND t1.id = i.id
		INNER JOIN [bitemp].[MyBiTempTable] t2 ON 1=1
			AND t2.id = t1.id
			AND (
				 -- 3, 4, 5, 6, 7, 8
				(ISNULL(t2.transBegin, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) <= ISNULL(t1.transBegin, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) AND
				 ISNULL(t2.transEnd,   CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')) > ISNULL(t1.transBegin,  CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')))
				OR
				 -- 9, 10, 11
				(ISNULL(t2.transBegin, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) > ISNULL(t1.transBegin, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) AND
				 ISNULL(t2.transBegin, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) < ISNULL(t1.transEnd,   CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')))
			) AND (
				(ISNULL(t2.validFrom, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) <= ISNULL(t1.validFrom, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) AND
				 ISNULL(t2.validTo,   CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')) > ISNULL(t1.validFrom,    CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')))
				OR
				(ISNULL(t2.validFrom, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) > ISNULL(t1.validFrom, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) AND
				 ISNULL(t2.validFrom, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) < ISNULL(t1.validTo,   CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')))
			)

	IF @overlaps > @entries BEGIN
		ROLLBACK;
		THROW 50000, N'Overlapping intervals', 0;
	END
END
GO

CREATE VIEW [dbo].MyBiTempTable WITH SCHEMABINDING AS
SELECT
	 id
	,myNum
	,myText
	,validFrom
	,validTo
FROM
	[bitemp].MyBiTempTable
WHERE
	transEnd IS NULL
GO

CREATE TRIGGER [dbo].MyBiTempTable_DML ON [dbo].[MyBiTempTable]
INSTEAD OF INSERT, UPDATE, DELETE
AS BEGIN
	IF (ROWCOUNT_BIG() = 0) RETURN; -- https://docs.microsoft.com/en-us/sql/t-sql/statements/create-trigger-transact-sql

	DECLARE @now DATETIME2(7) = SYSDATETIME();

	UPDATE t
	SET transEnd = @now
	FROM
		deleted d
		INNER JOIN [bitemp].[MyBiTempTable] t ON
			t.transEnd IS NULL AND
			t.validFrom = d.validFrom AND
			t.validTo = d.validTo AND
			t.id = d.id

	INSERT [bitemp].[MyBiTempTable] (id, myNum, myText, validFrom, validTo, transBegin, transEnd)
	SELECT
		 id
		,myNum
		,myText
		,validFrom
		,validTo
		,@now
		,NULL
	FROM
		inserted
END
GO

CREATE FUNCTION [dbo].MyBiTempTable_ValidAt_AsOf (@valid DATETIME2, @trans DATETIME2)
RETURNS TABLE
AS RETURN (
	SELECT
		 id
		,myNum
		,myText
		,@valid validAt
		,@trans asOf
	FROM
		[bitemp].MyBiTempTable
	WHERE
		ISNULL(transBegin, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) <= @trans AND
		ISNULL(transEnd,   CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')) >  @trans AND
		ISNULL(validFrom,  CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) <= @valid AND
		ISNULL(validTo,    CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')) >  @valid
)
GO

CREATE FUNCTION [dbo].MyBiTempTable_ValidAt (@valid DATETIME2)
RETURNS TABLE
AS RETURN (
	SELECT
		 id
		,myNum
		,myText
		,@valid validAt
		,ISNULL(transBegin, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) transBegin
		,ISNULL(transEnd,   CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')) transEnd
	FROM
		[bitemp].MyBiTempTable
	WHERE
		ISNULL(validFrom,  CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) <= @valid AND
		ISNULL(validTo,    CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')) >  @valid
)
GO

CREATE FUNCTION [dbo].MyBiTempTable_AsOf (@trans DATETIME2)
RETURNS TABLE
AS RETURN (
	SELECT
		 id
		,myNum
		,myText
		,@trans asOf
		,ISNULL(validFrom,  CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) validFrom
		,ISNULL(validTo,    CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')) validTo
	FROM
		[bitemp].MyBiTempTable
	WHERE
		ISNULL(transBegin, CONVERT(DATETIME2, '0001-01-01 00:00:00.0000000')) <= @trans AND
		ISNULL(transEnd,   CONVERT(DATETIME2, '9999-12-31 23:59:59.9999999')) >  @trans
)
GO
