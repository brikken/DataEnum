-- Token count: 31
CREATE TABLE [dbo].MyBiTempTable (
	id INT NOT NULL PRIMARY KEY,
	myNum DECIMAL(18,2) NULL,
	myText VARCHAR(200) NOT NULL
)
DTWITH (BITEMPORAL (BTSCHEMA = [bitemp]));
/*GO

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
	IF (@@ROWCOUNT_BIG = 0) RETURN; -- https://docs.microsoft.com/en-us/sql/t-sql/statements/create-trigger-transact-sql

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
*/