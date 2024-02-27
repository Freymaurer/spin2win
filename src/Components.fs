namespace App

open Feliz
open Feliz.Router
open Feliz.DaisyUI

type private Page =
| Spin2Win
| About
| NotFound

type private Role =
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
    | ADC         -> "crimson"
    | Jungle      -> "darkgreen"
    | Top         -> "orchid"
    | Mid         -> "dodgerblue"
    | Support     -> "gold"
    | Custom cus  -> "ivory"

[<AutoOpen>]
module private Helper = 

  module PageLiterals =
    let [<Literal>] About = "about"
    let [<Literal>] NotFound = "not_found"

  let log = fun o -> Browser.Dom.console.log o

  let parseUrl = function
      // matches #/ or #
      | [ ] ->  Page.Spin2Win
      // matches #/users or #/users/ or #users
      | [ PageLiterals.About ] -> Page.About
      // matches #/users/{userId}
      // matches everything else
      | _ -> NotFound

  let buildUrlSegments = function
    | Page.Spin2Win -> [||]
    | Page.About -> [|PageLiterals.About|]
    | Page.NotFound -> [|PageLiterals.NotFound|]

type private Wheel =
    [<ReactComponent>]
    static member Wheel(roles: Role list, users: string list) =
      Html.div [
        prop.className "wheel-container"
        prop.children [
          Html.div [prop.className "wheel-center"; prop.text "spin"]
          Html.div [
            prop.className "wheel"
            prop.style [style.custom("--n", roles.Length)]
            prop.children [
              for i in 1 .. roles.Length do
                let role = List.item (i-1) roles
                Html.div [
                  prop.className "wheel-element"
                  prop.style [
                    style.custom("--i",i)
                    style.custom("--clr",role.ToColor())
                  ]
                  prop.children [Html.span (string role)]
                ]
            ]
          ]
        ]
      ]

    [<ReactComponent>]
    static member InputAddUser(addUser: string -> unit) =
      let state, setState = React.useState("")
      let reset() = setState ""
      let addUser = fun s -> addUser s; reset()
      Daisy.formControl [
        Html.div [
            prop.className "relative"
            prop.children [
                Daisy.input [
                  input.bordered; input.secondary; prop.placeholder "new user"; prop.valueOrDefault state; 
                  prop.onChange(fun (s: string) -> setState s); 
                  prop.onKeyDown(key.enter, fun _ -> addUser state)
                ]
                Daisy.button.button [
                    button.secondary
                    prop.className "absolute top-0 right-0 rounded-l-none"
                    prop.text "+"
                    prop.onClick (fun _ -> addUser state;)
                ]
            ]
        ]
    ]
    static member RoleSelectItem(role: Role, isActive, add, rmv) =
      Html.li [
          prop.onClick (fun _ -> if isActive then rmv role else add role)
          prop.children [
            Html.a [
              prop.children [
                Html.p (string role); 
                if isActive then Html.i [
                  prop.className "fa-solid fa-check place-self-end"
                ]
              ]
            ]
          ]
      ]
    static member RoleSelect(roles, setRoles) =
      let isActive (roles: Role list) (role: Role) = List.contains role roles
      let add (role: Role) = role::roles |> setRoles
      let rmv (role: Role) = roles |> List.except [role] |> setRoles 
      Daisy.dropdown [
        dropdown.hover
        prop.children [
            Daisy.button.button [
                button.secondary
                prop.text "Roles"
            ]
            Daisy.dropdownContent [
                prop.className "p-2 shadow menu bg-base-100 rounded-box w-52"
                prop.tabIndex 0
                prop.children [
                    Daisy.menu [
                      for role in Role.Default() do
                        Wheel.RoleSelectItem(role, isActive roles role, add, rmv)
                    ]
                ]
            ]
        ]
    ]

    static member Main() =
      let (roles: Role list), setRoles= React.useState(Role.Default)
      let (users: string list), setUsers = React.useState([])
      Html.div [
        prop.className "size-full flex flex-col items-center gap-4"
        prop.children [
          Html.div [
            prop.className "flex flex-row gap-2"
            prop.children [
              Wheel.InputAddUser(fun s -> s::users |> setUsers)
              Wheel.RoleSelect(roles, setRoles)
            ]
          ]
          Wheel.Wheel(roles, users)
        ]
      ]

type Components =

    /// <summary>
    /// The simplest possible React component.
    /// Shows a header with the text Hello World
    /// </summary>
    [<ReactComponent>]
    static member HelloWorld() = Html.h1 "Hello World"

    /// <summary>
    /// A stateful React component that maintains a counter
    /// </summary>
    [<ReactComponent>]
    static member Counter() =
        let (count, setCount) = React.useState(0)
        Html.div [
            Html.h1 count
            Html.button [
                prop.onClick (fun _ -> setCount(count + 1))
                prop.text "Increment"
            ]
        ]

    /// <summary>
    /// A React component that uses Feliz.Router
    /// to determine what to show based on the current URL
    /// </summary>
    [<ReactComponent>]
    static member Router() =
        let (currentUrl, updateUrl) = React.useState(Helper.parseUrl(Router.currentUrl()))
        React.router [
            router.onUrlChanged (Helper.parseUrl >> updateUrl)
            router.children [
                Html.div [
                  prop.className "h-screen flex flex-col"
                  // theme.dark
                  prop.children [
                    Components.Navbar()
                    Components.ContentContainer [
                      match currentUrl with
                      | Page.Spin2Win -> Wheel.Main()
                      | Page.About -> Components.Counter()
                      | Page.NotFound -> Html.h1 "Not found"
                    ]
                    Components.Footer()
                  ]
                ]
            ]
        ]

    static member ContentContainer (content: ReactElement list): Fable.React.ReactElement =
        Daisy.hero [
            prop.className "size-full"
            prop.style [style.backgroundImageUrl "https://picsum.photos/id/1005/1600/1400"]
            prop.children [
                Daisy.heroContent [
                    prop.className "text-center text-neutral-content flex flex-auto size-full"
                    prop.children content
                ]
            ]
        ]

    static member Footer(): Fable.React.ReactElement =
        Daisy.footer [
            prop.className "p-1"
            footer.center
            prop.children [
                Html.aside [
                    Html.p "Made with Fable and F# ❤️"
                ]
            ]
        ]

    static member private NavbarNavItem(name: string, targetPage: Page) =
      Daisy.button.button [
        button.ghost
        prop.text name
        prop.onClick (fun _ -> (Helper.buildUrlSegments >> Router.navigate) targetPage )
      ]

    [<ReactComponent>]
    static member Navbar(): Fable.React.ReactElement = 
      Daisy.navbar [
        prop.children [
          Daisy.navbarStart [
            Components.NavbarNavItem("Spin2Win", Page.Spin2Win)
            Components.NavbarNavItem("About", Page.About)
          ]
          Daisy.navbarCenter [Html.span "With two icons"]
          Daisy.navbarEnd [
            Daisy.toggle [
                theme.controller
                prop.value "light"
            ]
          ]
        ]
      ]