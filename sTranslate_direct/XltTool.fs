module XltTool
    open System
    open System.Data
    open System.Data.Linq
    open FSharp.Data.TypeProviders
    open Microsoft.FSharp.Linq
    open FSharp.Configuration
    open XltEnums

    // Sets the database connection string
    type Settings = AppSettings<"App.config">
    type dbSchema = SqlDataConnection<ConnectionStringName = "dbConnectionString">

    // Language to translate from
    let FromLanguageCode = "en"

    //////////////////////////////////////////////////////////////////////////////////////////////
    //     GetToText function returns the translated string, if defined inn the Translate table. 
    //     If the fromText is not found, the value of fromText is returned unchanged.
    //     Multiple definitions for the same source string can be registered, and in case, 
    //     the property and context fields must be used to separate them. 
    let GetToText criteria (fromText : string) property (context : string) toLanguageCode =
        if fromText.Trim() = "" then "" |> ignore
        
        // Open a connection to the database
        use db = dbSchema.GetDataContext(Settings.ConnectionStrings.DbConnectionString)
        // Log database activity
        db.DataContext.Log <- System.Console.Out

        // If toLanguageCode is not valid, sets it to default "no"
        let toLang =
            match toLanguageCode with
            | null | "" -> "no"
            | _ -> toLanguageCode

        // Do search by criteria
        let coll = 
                query {
                    for x1 in db.Translation do
                    where (x1.Criteria.ToLower() = criteria.ToString().ToLower())
                    where (x1.FromLang = FromLanguageCode)
                    where (x1.FromText = fromText)
                    where (x1.Property.ToLower() = property.ToString().ToLower())
                    where (x1.Context.ToLower() = context.ToLower())
                    where (x1.ToLang = toLang)
                    select x1 }
        
        // Returns the ToText field from the first row in coll, if empty it returns the original text
        try
            let result = Seq.head coll
            result.ToText
        with _ -> fromText

    ////////////////////////////////////////////////////////////////////////////////////////////