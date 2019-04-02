grammar Configuration;

configuration
    : filter*
    ;

filter
    : NEWLINE #filterNone
    | not=NOT? server=partFilter ('.' database=partFilter ('.' schema=partFilter ('.' table=partFilter)?)?)? NEWLINE #filterSome
    ;

partFilter
    : WILDCARD #partFilterWildcard
    | id #partFilterId
    ;

id
    : name=ID
    | '[' name=.*? ']'
    ;

NOT : '!';
WILDCARD : '*';
ID : [a-zA-Z_][a-zA-Z0-9@#$_]*;
NEWLINE : [\r\n]+;
WS : [ \t]+ -> skip;
