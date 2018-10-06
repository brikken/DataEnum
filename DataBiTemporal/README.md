# DataBiTemporal
## Approaches
### Define everything
Tables are defined from the start.
Pros
- full control
- possibility for future vendor independence
Cons
- re-implement entire DML code generation
### Define add-on
Tables are assumed to exist. Only a bi-temporal add-on is defined.
Pros
- extending, not re-implementing the code generation engine
Cons
- less control
- makes pre-validations harder
