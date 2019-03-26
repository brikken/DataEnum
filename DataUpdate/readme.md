# Automatic data update
* Schema analysis
  * Iterate databases
    * Iterate tables
      * Properties
        * Has identity
        * In indexed view (see snippet)
      * Indexes
      * Triggers
      * Columns
      * Foreign key references
      * Walk and group foreign key references
* Optimization opportunities
  * Foundation: Roll forward state and schema the way it originally happened
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
      * Else: Load base tables first (ordered DELETE and INSERT)
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
* Manual overrides
  * Excludes based on filter
  * Manual entry at qualified location in processing order
* Error handling
  * Limit to dependent context (foreign key hierarchy)
  * Schema analysis traversal