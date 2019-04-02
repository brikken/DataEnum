// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

#load "Library1.fs"
open DataUpdateConfiguration

// Define your library scripting code here
let testConfig = [
    { Type = Positive; Definition = Server Wildcard }
    { Type = Negative; Definition = ServerDatabase (Named "MyDb", Wildcard)}
]
