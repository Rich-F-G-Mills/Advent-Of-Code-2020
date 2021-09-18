
open System
open System.IO
open System.Text.RegularExpressions
open FSharpx.Text.Regex
open FSharpx

[<AutoOpen>]
module Rules =

    type Rule =
        | Letter of Char
        | SubRules of int list
        | OrRules of int list * int list

    let rec checkMatch (rules: Map<int, Rule>) ruleno remaining =
        let bindFold =
            let binder curr_state subruleno =
                    Result.bind (checkMatch rules subruleno) curr_state

            List.fold binder (Ok remaining)

        match remaining with
        | [] -> Error "Nothing left of message."
        | r::rs ->
            match rules.[ruleno] with
            | Letter l when r = l ->
                Ok rs

            | Letter l ->
                Error $"Failed to match against {l}."

            | SubRules srs ->
                srs |> bindFold

            | OrRules (srs1, srs2) ->
                srs1
                |> bindFold
                |> function
                    | Error _ -> srs2 |> bindFold
                    | Ok _ as result -> result


[<EntryPoint>]
let main argv =
    
    let (rules, messages) =
        let inputs =
            File.ReadAllLines "INPUTS_TEST.TXT"

        let (_rules, _messages) =
            inputs
            |> Array.findIndex String.IsNullOrEmpty    
            |> Array.splitAt
            <| inputs
            |> function
                | (rs, ms) -> (rs, ms.[1..])

        let parseRule input =

            let (|MatchNoOpts|_|) = (|Match|_|) RegexOptions.None
            let (|MatchLetter|_|) = (|MatchNoOpts|_|) "^\\s\"(\w)\"$"
            let (|MatchSubRules|_|) = (|MatchNoOpts|_|) "^(?:\\s(\\d+))+$"
            let (|MatchOrRules|_|) = (|MatchNoOpts|_|) "^(?:\\s(\\d+))+\\s\\|(?:\\s(\\d+))+$"

            let extractSubRules (srs: Group) =
                srs.Captures
                |> Seq.map (fun x -> x.Value |> int32)
                |> Seq.toList

            let (ruleno, spec) =
                match tryMatch @"^(\d+):(.*)$" input with
                | Some { GroupValues = [ group_ruleno; group_spec ] }
                    -> (int32 group_ruleno, group_spec)

                | _ -> failwith $"Unable to process record number for rule '{input}'."

            let rule =
                match spec with
                | MatchLetter { Groups = [ letter ] }
                    -> Letter letter.Value.[0]

                | MatchSubRules { Groups = [ subrules ] }
                    -> SubRules (subrules |> extractSubRules)

                | MatchOrRules { Groups = [ subrules1; subrules2 ] }
                    -> OrRules (subrules1 |> extractSubRules, subrules2 |> extractSubRules)
                
                | otherwise -> failwith $"Unable to parse rule '{otherwise}'"

            (ruleno, rule)

        let rules =
            _rules
            |> Seq.map parseRule
            |> Map.ofSeq

        (rules, _messages)

    let checkAgainstRule0 rules msg =  
        msg
        |> Seq.map id
        |> Seq.toList
        |> checkMatch rules 0
        |> function | Ok [] -> true | _ -> false

    //messages
    //|> Seq.filter (checkAgainstRule0 rules)
    //|> Seq.length
    //|> printfn "Part 1 answer = %i\n\n"

    printfn "Messages satisfying part 1 rules...\n\n"

    messages
    |> Seq.filter (checkAgainstRule0 rules)
    |> Seq.iter (printfn "%A")

    let new_rules =
        rules
        |> Map.add 8 (OrRules ([42], [42; 8]))
        |> Map.add 11 (OrRules ([42; 31], [42; 11; 31]))

    printfn "\n\nMessages satisfying part 2 rules...\n\n"

    //let test = checkMatch new_rules 0 ("babbbbaabbbbbabbbbbbaabaaabaaa" |> List.ofSeq)

    messages
    |> Seq.filter (checkAgainstRule0 new_rules)
    |> Seq.iter (printfn "%A")

    printfn "\n\n"

    0
