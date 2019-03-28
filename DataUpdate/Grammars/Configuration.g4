grammar Configuration;

configuration
    : filterExpr*
    ;

filterExpr
    : not=NOT?
    ;

NOT :   '!';
