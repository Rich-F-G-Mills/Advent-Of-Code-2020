
open System
open System.IO

open FSharpx.Text.Regex


type Instruction =
    | SetPlayer1
    | SetPlayer2
    | IssuedCard of Value: int
    | Ignore

type Player =
    | Neither
    | Player1
    | Player2


let parseLines lines =
    let (|IsInt|_|) (str: string) =
        match Int32.TryParse str with
        | true, value -> Some value
        | _ -> None

    let (|IsPlayer|_|) playerId =
        function
        | Match Options.None "^Player (\d)" { GroupValues = [IsInt playerId'] }
            when playerId = playerId' -> Some ()
        | _ -> None

    let (|IsCard|_|) =
        function
        | IsInt value -> Some value
        | _ -> None

    let parseLine =
        function
        | IsPlayer 1 -> SetPlayer1
        | IsPlayer 2 -> SetPlayer2
        | IsCard value -> IssuedCard value
        | _ -> Ignore

    let rec loop currentPlayer player1Cards player2Cards instrs =
        match instrs, currentPlayer with
        | SetPlayer1::instrs, _ ->
            loop Player1 player1Cards player2Cards instrs

        | SetPlayer2::instrs, _ ->
            loop Player2 player1Cards player2Cards instrs

        | IssuedCard value::instrs, Player1 ->
            loop currentPlayer (value::player1Cards) player2Cards instrs

        | IssuedCard value::instrs, Player2 ->
            loop currentPlayer player1Cards (value::player2Cards) instrs

        | IssuedCard _::_, Neither ->
            failwith "Cannot issue card when a player has yet to be specified."

        | [], _ ->
            (List.rev player1Cards, List.rev player2Cards)

        | Ignore::instrs, _ ->
            loop currentPlayer player1Cards player2Cards instrs

    lines
    |> List.map parseLine
    |> loop Neither [] []
            
        
let rec playGame (player1Cards: int list, player2Cards: int list) =
    match player1Cards, player2Cards with
    | p1c::p1cs, p2c::p2cs when p1c > p2c ->
        playGame (List.append p1cs [p1c; p2c], p2cs)
    | p1c::p1cs, p2c::p2cs when p1c < p2c ->
        playGame (p1cs, List.append p2cs [p2c; p1c])
    | p1c::_, p2c::_ when p1c = p2c ->
        failwith "Player cards are equal."
    | [], _ | _, [] -> (player1Cards, player2Cards)
    | _ -> failwith "Unexpected error."
            


[<EntryPoint>]
let main argv =
    let (player1Cards, player2Cards) =
        File.ReadAllLines "INPUTS.TXT"
        |> List.ofArray
        |> parseLines

    let (player1EndDeck, player2EndDeck) =
        playGame (player1Cards, player2Cards)

    List.append player1EndDeck player2EndDeck
    |> List.rev
    |> List.indexed
    |> List.map (fun (idx, v) -> v * (idx+1))
    |> List.sum
    |> printfn "Part 1 asnwer = %i"

    0

