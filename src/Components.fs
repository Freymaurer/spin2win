﻿namespace App

open Fable.Core
open Feliz
open Feliz.Router
open Feliz.DaisyUI
open Model
open Routing

type private Wheel =

    [<ReactComponent>]
    static member InputUserName(user: string, setUser: string -> unit) =
      Daisy.formControl [
        prop.className "grow"
        prop.children [
            Daisy.input [
                prop.className "grow"
                input.bordered; input.secondary; prop.placeholder "new user"; prop.valueOrDefault user; 
                prop.onChange(fun (s: string) -> setUser s); 
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

    static member RoleSelect(roles, setRoles, users: string list, setUsers) =
      let isActive (roles: Role list) (role: Role) = List.contains role roles
      let add (role: Role) = 
        role::roles |> setRoles
        ""::users |> setUsers
      let rmv (role: Role) = 
        let i = roles |> List.findIndex (fun r -> r = role)
        roles |> List.removeAt i |> setRoles
        users |> List.removeAt i |> setUsers
      Daisy.dropdown [
        prop.style [style.zIndex 2]
        dropdown.hover
        prop.children [
            Daisy.button.button [
                button.secondary
                button.wide
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

    [<ReactComponent>]
    static member SpinButton(spin: unit -> unit) =
        Daisy.button.button [
            prop.style [style.position.absolute; style.top (length.perc 50); style.left (length.perc 50); style.transform.translate (length.perc -50, length.perc -50)]
            button.circle
            button.lg
            button.primary
            prop.text "SPIN"
            prop.onClick(fun _ -> spin())
        ]

    [<ReactComponent>]
    static member Main() =
      let (roles: Role list), setRoles = React.useState(LocalStorage.Role.read)
      let (users: string list), setUsers = React.useState(LocalStorage.Users.read (roles.Length))
      let setRoles = fun roles ->
        LocalStorage.Role.write roles
        setRoles roles
      let setUsers = fun users ->
        LocalStorage.Users.write users
        setUsers users
      let spin users = 
        let nextUsers = Randomizer.randomize users
        setUsers nextUsers
      Html.div [
        prop.className "size-full flex flex-col items-center gap-4 lg:flex-row"
        prop.children [
          Html.div [
            prop.className "flex-grow card gap-2 flex"
            prop.children [
              Wheel.RoleSelect(roles, setRoles, users, setUsers)
              for i in 0 .. (users.Length-1) do
                let user = users.[i]
                let setUser = fun s -> users |> List.mapi (fun li u -> if li = i then s else u) |> setUsers
                Wheel.InputUserName(user, setUser)
            ]
          ]
          //Daisy.divider [prop.className "lg:divider-horizontal"]
          Html.div [
            prop.className "grid flex-grow card rounded-box place-items-center"
            prop.style [style.width (length.perc 100); style.height 500; style.position.relative]
            prop.children [
                Components.Wheel.Main(roles, users)
                Wheel.SpinButton(fun () -> spin users)
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
                  prop.children [
                    Html.div [ prop.style [style.position.fixedRelativeToWindow; style.left 0; style.right 0; style.top 0; style.bottom 0; style.backgroundImageUrl "./img/bc1.png"; style.backgroundRepeat.noRepeat; style.backgroundSize.cover; style.zIndex -1]]
                    Html.div [ prop.style [style.position.fixedRelativeToWindow; style.left 0; style.right 0; style.top 0; style.bottom 0; style.custom ("background", "linear-gradient(45deg, #0B1C38, #000, #0B1C38)"); style.opacity 0.8; style.zIndex -1] ]
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
          //Daisy.navbarCenter [Html.span "With two icons"]
          Daisy.navbarEnd [
            //Daisy.toggle [
            //    theme.controller
            //    prop.value "light"
            //]
          ]
        ]
      ]