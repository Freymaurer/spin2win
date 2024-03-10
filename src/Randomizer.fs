module Randomizer

open System
open Model

let private shuffleSequence<'a> (rng: Random) (l: 'a list) =
    l |> List.sortBy (fun _ -> rng.Next())

let randomize (users: string list) =
    let rnd = System.Random()
    shuffleSequence rnd users
    // shuffleSequence rnd roles