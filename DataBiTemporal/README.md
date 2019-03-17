# DataBiTemporal

## Project

### Useful extensions
* ANTLR Language Support
* StringTemplate 4 Language Support
* Markdown Editor

## ToDo
Fix the `null` issue in the template.

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
`DTWITH` is inspired by the T-SQL syntax `WITH` where `DT` is short for Data Temporality

### Define add-on
Tables are assumed to exist. Only a bi-temporal add-on is defined.

Pros
- extending, not re-implementing the code generation engine

Cons
- less control
- makes pre-validations harder

## Things to consider

### Primary key
The bi-temporal update trigger relies heavily on a primary key. This is used both to update the transaction table, but also to check for overlapping validity intervals. Are there any requirements to the primary key that must be fulfilled for this to work? Primary keys can also be defined both inline or as a constraint "on top".

### Performance
Data is only stored once in the bi-temporal table. When retrieving the current state, all rows with `transEnd IS NULL` is retrieved. Consider allowing specifiying performance improvements by storing current data in a 
more accessible way.