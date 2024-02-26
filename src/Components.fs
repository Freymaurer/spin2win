namespace App

open Feliz
open Feliz.Router
open Feliz.DaisyUI

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
        let (currentUrl, updateUrl) = React.useState(Router.currentUrl())
        React.router [
            router.onUrlChanged updateUrl
            router.children [
                Html.div [
                  // theme.dark
                  prop.children [
                    Components.Navbar()
                    match currentUrl with
                    | [ ] -> Html.h1 "Index"
                    | [ "hello" ] -> Components.HelloWorld()
                    | [ "counter" ] -> Components.Counter()
                    | otherwise -> Html.h1 "Not found"
                  ]
                ]
            ]
        ]

    [<ReactComponent>]
    static member Navbar(): Fable.React.ReactElement = 
      Daisy.navbar [
        prop.children [
          Daisy.navbarStart [
            Daisy.button.button [
                button.square
                button.ghost
                prop.children [
                    Html.i [ prop.className "fas fa-bars" ]
                ]
            ]
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