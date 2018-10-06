# DataBiTemporal

## Approaches

### Define everything
Tables are defined from the start.

Pros
- full control
- possibility for future vendor independence

Cons
- re-implement entire DML code generation

#### Sample Syntax
```SQL
CREATE TABLE [dbo].[MyBiTempTable] (
	id INT NOT NULL PRIMARY KEY,
	myNum DECIMAL(18,2) NULL,
	myText VARCHAR(200) NOT NULL
)
DTWITH (BITEMPORAL (BTSCHEMA = [bitemp]))
```

### Define add-on
Tables are assumed to exist. Only a bi-temporal add-on is defined.

Pros
- extending, not re-implementing the code generation engine

Cons
- less control
- makes pre-validations harder
