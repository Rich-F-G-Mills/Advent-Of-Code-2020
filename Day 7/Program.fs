// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Text.RegularExpressions

open FParsec

let bag_target =
    manyChars asciiLetter
    .>> spaces
    .>>. manyChars asciiLetter
    |>> (fun x -> $"{fst x} {snd x}")

let contains_no_bags =
    pstring "no other bags"
    >>% []

let sub_bags =
    pint32
    .>> spaces
    .>>. bag_target
    .>> spaces
    .>> pstring "bag"
    .>> optional (pchar 's')

let contains_other_bags =
    sepBy sub_bags (pstring ", ")

let rule =
    bag_target
    .>> pstring " bags contain "
    .>>. (contains_no_bags <|> contains_other_bags)
    .>> pchar '.'
    .>> eof
    

[<EntryPoint>]
let main argv =
    let rules =
        File.ReadAllLines "Inputs.txt"
        |> Array.map (run rule)
        |> Array.map (function | Success (r, _, _) -> r
                               | _ -> failwith "Unexpected error.")
        |> Map.ofArray

    let rule_links =
        rules
        |> Map.map (fun _ precedents -> precedents |> List.map snd |> Set.ofList)
        |> Map.toList

    let rec find_deps (found: string Set) (to_find: string Set) =
        let new_found =
            rule_links
            |> List.choose (fun (bag, contains) ->
                    if Set.intersect to_find contains |> Set.isEmpty then None
                    else Some bag
                )
            |> Set.ofList    
        
        let new_not_already_found =
            Set.difference new_found found

        if new_not_already_found.IsEmpty then
            found
        else
            find_deps (Set.union found new_not_already_found) new_not_already_found


    let rec sort_rules (sorted: string list) (not_sorted: (string * (string Set)) list) =
        if not_sorted.IsEmpty then
            sorted

        else
            let next_found = 
                not_sorted
                |> List.find (
                    fun (bag, deps) ->
                        deps |> Set.forall (fun x -> List.contains x sorted))

            sort_rules ((fst next_found)::sorted) (not_sorted |> List.except [next_found])

    
    find_deps Set.empty (set ["shiny gold"])
    |> Set.count
    |> printfn "Part 1 answer = %i"

    let sorted_rules =
        List.foldBack (
            fun bag (state: Map<string, int>) ->
                let curr_total =
                    rules.[bag]
                    |> List.sumBy (fun (count, sub_bag) -> count * (1 + state.[sub_bag]))

                state
                |> Map.add bag curr_total) (sort_rules [] rule_links) Map.empty<string, int>
     
    sorted_rules.["shiny gold"]
    |> printfn "Part 2 answer = %i"

    0 // return an integer exit code
