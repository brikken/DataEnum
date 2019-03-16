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
	:	T_INT			#colTypeInt
	|	T_VARCHAR		#colTypeVarchar
	|	T_DECIMAL		#colTypeDecimal
	;

colOpt
	:	NULL			#colOptNull
	|	NOT NULL		#colOptNotNull
	|	PRIMARY KEY		#colOptPrimaryKey
	;

tabOpt
	:	DTWITH '(' opts+=dtwithOpt (',' opts+=dtwithOpt)* ')'		#tabOptDtWith
	;

dtwithOpt
	:	BITEMPORAL ('(' opts+=btOpt (',' opts+=btOpt)* ')')?		#dtWithOptBiTemporal
	;

btOpt
	:	BTSCHEMA '=' sch=OBJ_ID		#btOptBtSchema
	;

/*
 * Lexer Rules
 */

fragment A : [aA]; // match either an 'a' or 'A'
fragment B : [bB];
fragment C : [cC];
fragment D : [dD];
fragment E : [eE];
fragment F : [fF];
fragment G : [gG];
fragment H : [hH];
fragment I : [iI];
fragment J : [jJ];
fragment K : [kK];
fragment L : [lL];
fragment M : [mM];
fragment N : [nN];
fragment O : [oO];
fragment P : [pP];
fragment Q : [qQ];
fragment R : [rR];
fragment S : [sS];
fragment T : [tT];
fragment U : [uU];
fragment V : [vV];
fragment W : [wW];
fragment X : [xX];
fragment Y : [yY];
fragment Z : [zZ];

CREATE		:	C R E A T E;
TABLE		:	T A B L E;
T_INT		:	I N T;
T_VARCHAR	:	V A R C H A R '(' (M A X | [0-9]+) ')';
T_DECIMAL	:	D E C I M A L '(' [0-9]+ ',' [0-9]+ ')';
NOT			:	N O T;
NULL		:	N U L L;
PRIMARY		:	P R I M A R Y;
KEY			:	K E Y;
DTWITH		:	D T W I T H;
BITEMPORAL	:	B I T E M P O R A L;
BTSCHEMA	:	B T S C H E M A;

OBJ_ID
	:	[a-zA-Z_][a-zA-Z_0-9]*
	|	'[' .*? ']'
	;

C_INLINE	:	'--' ~[\r\n]* -> channel(HIDDEN);
C_BLOCK		:	'/*' .*? '*/' -> channel(HIDDEN);
WS			:	[ \r\n\t]+ -> channel(HIDDEN);
