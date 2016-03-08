namespace sTranslate_direct
module XltTool =
    open System
    open System.Data
    open System.Data.Linq
    open FSharp.Data.TypeProviders
    open Microsoft.FSharp.Linq
    open FSharp.Configuration
    open XltEnums
    open Model

    // Sets the database connection string
    type Settings = AppSettings<"App.config">
    type dbSchema = SqlDataConnection<ConnectionStringName = "dbConnectionString">

    // Language to translate from
    let FromLanguageCode = "en"
    
    // Copy the contents of a database row into a record type
    let toTranslation (xlt : dbSchema.ServiceTypes.Translation) =
        {
            Id = xlt.Id
            FromText = xlt.FromText
            ToText = xlt.ToText
            Context = xlt.Context
            Property = xlt.Property
            Criteria = xlt.Criteria
            FromLang = xlt.FromLang
            ToLang = xlt.ToLang
        }

    let mutable _translateColl : List<Translation> = []

    let GetTranslations (reRead : bool) =
        if _translateColl = [] || reRead = true then
            use db = dbSchema.GetDataContext(Settings.ConnectionStrings.DbConnectionString)
            _translateColl <- 
            query {
                for xl in db.Translation do 
                    select xl
            } |> Seq.toList |> List.map toTranslation


    //////////////////////////////////////////////////////////////////////////////////////////////
    //     GetToText function returns the translated string, if defined inn the Translate table. 
    //     If the fromText is not found, the value of fromText is returned unchanged.
    //     Multiple definitions for the same source string can be registered, and in case, 
    //     the property and context fields must be used to separate them. 
    let GetToText criteria (fromText : string) property (context : string) toLanguageCode =
        if fromText.Trim() = "" then ""
        else
        
        // If toLanguageCode is not valid, sets it to default "no"
        let toLang =
            match toLanguageCode with
            | null | "" -> "no"
            | _ -> toLanguageCode

        // Open a connection to the database
        use db = dbSchema.GetDataContext(Settings.ConnectionStrings.DbConnectionString)
        
        // Do search by criteria
        let coll = 
            query {
                for xl in db.Translation do
                    where (xl.Criteria.ToLower() = criteria.ToString().ToLower()    &&
                        xl.FromLang = FromLanguageCode                              &&
                        xl.FromText = fromText                                      &&
                        xl.Property.ToLower() = property.ToString().ToLower()       &&
                        xl.Context.ToLower() = context.ToLower()                    &&
                        xl.ToLang = toLang)
                    select xl }
        
        // Returns the ToText field from the first row in coll, if empty it returns the original text
        try
            let result = Seq.head coll
            result.ToText
        with _ -> fromText

    ////////////////////////////////////////////////////////////////////////////////////////////
    let ToText criteria (fromText : string) property (context : string) toLanguageCode =
        if fromText.Trim() = "" then ""
        else

        // If toLanguageCode is not valid, sets it to default "no"
        let toLang =
            match toLanguageCode with
            | null | "" -> "no"
            | _ -> toLanguageCode
        
        GetTranslations false
        
        let mutable result = fromText
        for xl in _translateColl do
            if  xl.Criteria.ToLower() = criteria.ToString().ToLower() && 
                xl.FromLang = FromLanguageCode && xl.FromText = fromText && 
                xl.Property.ToLower() = property.ToString().ToLower() && 
                xl.Context.ToLower() = context.ToLower() && 
                xl.ToLang = toLanguageCode 
            then
                result <- xl.ToText

        result
            