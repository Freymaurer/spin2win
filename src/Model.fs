namespace Model

type Role =
| ADC
| Jungle
| Top
| Mid
| Support
| Custom of string
  static member fromString = function
    | "ADC"     -> ADC
    | "Jungle"  -> Jungle
    | "Top"     -> Top
    | "Mid"     -> Mid
    | "Support" -> Support
    | anyElse -> Custom anyElse
  static member Default() = [Top; Jungle; Mid; ADC; Support]
  member this.ToColor() =
    match this with
    | Top         -> "oklch(var(--wa))"
    | Jungle      -> "oklch(var(--su))"
    | Mid         -> "oklch(var(--in))"
    | ADC         -> "oklch(var(--p))"
    | Support     -> "oklch(var(--er))"
    | Custom cus  -> "#00C49F"

    /// <summary>
    /// This function is only used for development and provides default user names for ui testing.
    /// </summary>
    member this.ToUserName() =
        match this with
        | Top         -> "Patrick"
        | Jungle      -> "Kevin"
        | Mid         -> "Lena"
        | ADC         -> "Chris"
        | Support     -> "Anne/Caro"
        | Custom cus  -> "#00C49F"
        
