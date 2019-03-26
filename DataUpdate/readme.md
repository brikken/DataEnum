# Automatic data update
Foundation: Roll forward state and schema the way it originally happened
* Optimization opportunities
  * Islands (MERGE), ie. no
    * Identity columns
    * Foreign keys (outgoing)
    * Membership of indexed view
    * Triggers
  * Mitigations
    * Set identity insert on
    * Disable triggers
    * Foreign keys
      * Self-referencing: By row, initialize with fk=id or fk is null
      * Else load base tables first (ordered DELETE and INSERT)
    * Disable indexes on views
* Actions
  * Allow custom listener modules to modify actions descriptors
    * Cascading
    * Save copies of action descriptors before modification (ICloneable)
    * Example
        * &lt; x mb: everything
        * Else
            * Has status date: Grab latest n dates
            * Else
                * &lt; 5x mb: everything
                * Else: manual filter
* Processing order
  * Database
    * Foreign key hierarchies (stable hash)
      * Schema
      * Table
    * Other members
      * Schema
      * Table
* Cascading filters (include and exclude)
* Manual overrides
  * Excludes based on filter
  * Manual entry at qualified location in processing order
* Error handling
  * Limit to dependent context (foreign key hierarchy)
* Schema analysis
  * Iterate databases
    * Iterate tables
      * Create and save properties (identity, indexed view, trigger)
      * Set foreign key references
      * Walk and group foreign key references
