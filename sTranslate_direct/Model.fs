namespace sTranslate_direct
module Model =

    type Translation =
        {
            Id : int
            FromText : string
            ToText : string
            Context : string
            Property : string
            Criteria : string
            FromLang : string
            ToLang : string
        }