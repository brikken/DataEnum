module File1
open DataUpdateConfiguration
open System

//let testConfig = Configuration [
//    { Type = Positive; Definition = Server (ServerRef ElementReference.Wildcard) }
//    { Type = Negative; Definition = ServerDatabase (ServerRef (Named "MyDb"), DatabaseRef ElementReference.Wildcard)}
//]

let applyFilter filter elements =
    match filter with
    | { Type = t; Definition = Server (ServerFilter.Wildcard) } ->
        match t with
        | Positive -> elements
        | Negative -> []
    | { Type = t; Definition = Server (ServerFilter.Name name) } ->
        let predicate =
            match t with
            | Positive -> (=)
            | Negative -> (<>)
        List.filter (predicate name) elements

