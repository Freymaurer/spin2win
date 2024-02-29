namespace App

open Fable.Core
open Feliz
open Feliz.Router
open Feliz.DaisyUI
open Model
open Routing
open Fable.Core.JsInterop
open System

type private Wheel =

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
        prop.style [style.zIndex 2]
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
      let (roles: Role list), setRoles= React.useState(Role.Default())
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
          Html.div [
            prop.className "glass"
            prop.style [style.width (length.perc 100); style.height 500;]
            prop.children [
                Components.Wheel.Main(roles, users)
            ]
          ]
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
        let (currentUrl, updateUrl) = React.useState(parseUrl(Router.currentUrl()))
        React.router [
            router.onUrlChanged (parseUrl >> updateUrl)
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
        prop.onClick (fun _ -> (buildUrlSegments >> Router.navigate) targetPage )
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