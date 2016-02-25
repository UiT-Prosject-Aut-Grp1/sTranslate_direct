module Enums
    open System

    //////////////////////////
    // Get enumeration state
    let GetEnumState myType (value : string) =
        
        // Exits the function if the input is empty or null
        if value = null || value = "" then
            eprintf "%s:GetEnumState: Mandatory value parameter must be entered in call" (myType.ToString()) |> ignore
            exit 1
        
        // Filters a string array and finds the correct Enumeration
        let enumName = Enum.GetNames(myType) |> Seq.filter (fun x -> x.ToLower() = value.ToLower()) |> Seq.head

        // Exits if there was no match
        if enumName = null then
            eprintf "%s:GetEnumState: Enumeration don't contain value '%s'" (myType.ToString()) value |> ignore
            exit 1
        
        // Returns the object of the specified type with the correct state
        Enum.Parse(myType, enumName)