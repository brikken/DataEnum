namespace DataUpdateConfiguration

open System.Runtime.Serialization.Formatters

type FilterType =
    | Positive
    | Negative

//type ElementReference =
//    | Wildcard
//    | Named of string

type ServerName = ServerName of string
type ServerFilter =
    | Wildcard
    | Name of ServerName

type DatabaseName = DatabaseName of string
type DatabaseFilter =
    | Wildcard
    | Name of DatabaseName

//type ServerReference =
//    | ServerRef of ElementReference
//type DatabaseReference =
//    | DatabaseRef of ElementReference
//type SchemaReference =
//    | ElementReference
//type TableReference =
//    | ElementReference

type FilterDefinition =
    | Server of ServerFilter
    | ServerDatabase of ServerFilter * DatabaseFilter
    //| ServerDatabaseSchema of ServerReference * DatabaseReference * SchemaReference
    //| ServerDatabaseSchemaTable of ServerReference * DatabaseReference * SchemaReference * TableReference

type Filter = {
    Type: FilterType
    Definition: FilterDefinition
}

type Configuration = Configuration of Filter list
