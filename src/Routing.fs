namespace Routing

type Page =
| Spin2Win
| About
| NotFound

[<AutoOpen>]
module Routing = 

  module PageLiterals =
    let [<Literal>] About = "about"
    let [<Literal>] NotFound = "not_found"

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