
open System
open System.IO
open System.Reflection
open XltEnums
open XltTool

[<EntryPoint>]
let TestApp argv = 
    // Print arguments for debugging
    printfn "%A" argv

    // Get time
    let startTime = DateTime.Now
    
    // Get my name
    let strArr = Assembly.GetCallingAssembly().FullName.Split(',')
    let myName = strArr.[0]
    
    // Exit if too few arguments
    if Array.length argv < 5 then
        eprintf "Incorrect number of arguments!"
        exit 1

    let context = argv.[0]
    let property = ToPropertyType argv.[1]
    let criteria = ToCriteria argv.[2]
    let toLang = argv.[3]
    let text = argv.[4]
    
    // Calls translation and prints output  
    let toText = GetToText criteria text property context toLang
    printfn "%s: English: \"%s\" translated to Norwegian: \"%s\"" myName text toText

    // Find elapsed time
    let elapsedTime = DateTime.Now.Subtract(startTime)
    printfn "%s: Duration: %A" myName elapsedTime

    Console.ReadKey() |> ignore
    0

