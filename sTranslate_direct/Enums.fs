namespace sTranslate_direct
module Enums =
    open System

    //////////////////////////
    // Get enumeration state
    let GetEnumState myType (value : string) =
        
        // Exits the function if the input is empty or null
        if value = null || value = "" then
            eprintf "%s:GetEnumState: Mandatory value parameter must be entered in call" (myType.ToString()) |> ignore
            exit 1
        
        try
            // Filters a string array and finds the correct Enumeration
            let enumName = Enum.GetNames(myType) |> Seq.filter (fun x -> x.ToLower() = value.ToLower()) |> Seq.head
            // Returns the object of the specified type with the correct state
            Enum.Parse(myType, enumName)
        with _ ->
            // Exits if there was no match
            eprintf "%s:GetEnumState: Enumeration don't contain value '%s'" (myType.ToString()) value |> ignore
            exit 1