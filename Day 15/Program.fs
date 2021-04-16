// Learn more about F# at http://fsharp.org

open System
open System.Collections.Generic
open System.IO

type NumberLocation =
    | NonePlayedBefore
    | OnePlayedBefore of location: int
    | TwoOrMorePlayedBefore of location: int * location2: int

type GameState =
    | State of last_played: int * num_played: int * locations: NumberLocation array

let play_number (State (last_played, num_played, locations)) number =
    locations.[number] <-
        match locations.[number] with
        | NonePlayedBefore ->
            OnePlayedBefore (1 + num_played)
        | OnePlayedBefore location ->
            TwoOrMorePlayedBefore (1 + num_played, location)
        | TwoOrMorePlayedBefore (location, _) ->
            TwoOrMorePlayedBefore (1 + num_played, location)

    State (number, num_played + 1, locations)
    

let iterate_game (State (last_played, num_played, locations) as state) =     
    match locations.[last_played] with
    | NonePlayedBefore -> failwith "Unexpected error!"
    | OnePlayedBefore _ -> play_number state 0
    | TwoOrMorePlayedBefore (location, location2) -> play_number state (location - location2)
    
let iterate_for_fold state (idx: int) =
    iterate_game state
    

[<EntryPoint>]
let main argv =
    let starting_state_part1 =
        [5;2;8;16;18;0;1]
        |> List.fold play_number (State (-1, 0, Array.init 10_000 (fun _ -> NonePlayedBefore)))      
    
    Seq.fold iterate_for_fold starting_state_part1 {8..2_020}
    |> function | State (last_played, _, _) -> last_played
    |> printfn "Part 1 answer = %i"

    let starting_state_part2 =
        [5;2;8;16;18;0;1]
        |> List.fold play_number (State (-1, 0, Array.init 30_000_000 (fun _ -> NonePlayedBefore)))      
    
    Seq.fold iterate_for_fold starting_state_part2 {8..30_000_000}
    |> function | State (last_played, _, _) -> last_played
    |> printfn "Part 2 answer = %i"

    0 // return an integer exit code
