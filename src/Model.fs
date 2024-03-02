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
        
