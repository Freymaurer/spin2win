namespace LocalStorage

open Browser
open Fable.SimpleJson

module Users =

  let [<Literal>] Key = "USERS"

  let _default = []

  let write (users: string list) =
    let jsonString = Json.serialize users
    WebStorage.localStorage.setItem(Key, jsonString)

  let read (roleCount: int) =
    let jsonString = WebStorage.localStorage.getItem(Key)
    let localStorageList =
      match isNull jsonString with
      | true -> _default
      | false ->
        let userList = Json.tryParseAs<string list>(jsonString)
        match userList with
        | Ok users -> users
        | Error _ -> _default
    if localStorageList.Length <> roleCount then
      [for _ in 0 .. roleCount-1 do yield ""]
    else
      localStorageList

module Role =

  open Model
  let [<Literal>] Key = "Roles"

  let _default = Role.Default()

  let write (roles: Role list) =
    let jsonString = Json.serialize roles
    WebStorage.localStorage.setItem(Key, jsonString)

  let read() =
    let jsonString = WebStorage.localStorage.getItem(Key)
    match isNull jsonString with
    | true -> _default
    | false ->
      let list = Json.tryParseAs<Model.Role list>(jsonString)
      match list with
      | Ok roles -> roles
      | Error _ -> _default