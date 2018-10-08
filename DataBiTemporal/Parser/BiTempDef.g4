grammar BiTempDef;

/*
 * Parser Rules
 */

compileUnit
	:	tableDef* EOF
	;

tableDef
	:	CREATE TABLE (db=OBJ_ID '.' sch=OBJ_ID '.' | sch=OBJ_ID '.')? tab=OBJ_ID '(' cols+=colDef (',' cols+=colDef)* ')' opts+=tabOpt* ';'?
	;

colDef
	:	col=OBJ_ID type=colType opts+=colOpt*
	;

colType
	:	T_INT
	|	T_VARCHAR
	|	T_DECIMAL
	;

colOpt
	:	NULL			#colOptNull
	|	NOT NULL		#colOptNotNull
	|	PRIMARY KEY		#colOptPrimaryKey
	;

tabOpt
	:	DTWITH '(' opts+=dtwithOpt (',' opts+=dtwithOpt)* ')'
	;

dtwithOpt
	:	BITEMPORAL ('(' opts+=btOpt (',' opts+=btOpt)* ')')?
	;

btOpt
	:	BTSCHEMA '=' sch=OBJ_ID
	;

/*
 * Lexer Rules
 */

CREATE		:	[Cc][Rr][Ee][Aa][Tt][Ee];
TABLE		:	[Tt][Aa][Bb][Ll][Ee];
T_INT		:	[Ii][Nn][Tt];
T_VARCHAR	:	[Vv][Aa][Rr][Cc][Hh][Aa][Rr] '(' ([Mm][Aa][Xx] | [0-9]+) ')';
T_DECIMAL	:	[Dd][Ee][Cc][Ii][Mm][Aa][Ll] '(' [0-9]+ ',' [0-9]+ ')';
NOT			:	[Nn][Oo][Tt];
NULL		:	[Nn][Uu][Ll][Ll];
PRIMARY		:	[Pp][Rr][Ii][Mm][Aa][Rr][Yy];
KEY			:	[Kk][Ee][Yy];
DTWITH		:	[Dd][Tt][Ww][Ii][Tt][Hh];
BITEMPORAL	:	[Bb][Ii][Tt][Ee][Mm][Pp][Oo][Rr][Aa][Ll];
BTSCHEMA	:	[Bb][Tt][Ss][Cc][Hh][Ee][Mm][Aa];

OBJ_ID
	:	[a-zA-Z_][a-zA-Z_0-9]*
	|	'[' .*? ']'
	;

C_INLINE	:	'--' ~[\r\n]* -> channel(HIDDEN);
C_BLOCK		:	'/*' .*? '*/' -> channel(HIDDEN);
WS			:	[ \r\n\t]+ -> channel(HIDDEN);
