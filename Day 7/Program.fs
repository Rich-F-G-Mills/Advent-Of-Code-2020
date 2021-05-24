
open System.IO
open FParsec

// Here we use FParsec rules in order to interprate the puzzle inputs.
let bagTarget =
    manyChars asciiLetter
    .>> spaces
    .>>. manyChars asciiLetter
    |>> (fun x -> $"{fst x} {snd x}")

let containsNoBags =
    pstring "no other bags"
    >>% []

let subBags =
    pint32
    .>> spaces
    .>>. bagTarget
    .>> spaces
    .>> pstring "bag"
    .>> optional (pchar 's')

let containsOtherBags =
    sepBy subBags (pstring ", ")

// This is the top-level grammar requirement.
let rule =
    bagTarget
    .>> pstring " bags contain "
    .>>. (containsNoBags <|> containsOtherBags)
    .>> pchar '.'
    .>> eof
    

[<EntryPoint>]
let main argv =
    let rules =
        // Convert the puzzle inputs into a mapping from bag name to a list of the
        // number and type of bags that need to go into it.
        File.ReadAllLines "Inputs.txt"
        |> Array.map (run rule)
        |> Array.map (function | Success (r, _, _) -> r
                               | _ -> failwith "Unexpected error.")
        |> Map.ofArray

    // For each bag, get the bag types that it depends on.
    let ruleLinks =
        rules
        |> Map.map (fun _ precedents -> precedents |> List.map snd |> Set)

    // This finds all bags that ultimately require any of those contained within 'to_find'.
    let rec findDeps (found: string Set) (toFind: string Set) =
        
        let newFound =
            ruleLinks
            |> Map.toSeq
            |> Seq.choose (fun (bag, contains) ->
                    if Set.intersect toFind contains |> Set.isEmpty then None
                    else Some bag
                )
            |> Set
        
        // Only include those which we haven't already found.
        let newNotAlreadyFound =
            Set.difference newFound found

        // If we haven't found any new bags then we're done.
        if newNotAlreadyFound.IsEmpty then
            found
        // Otherwise, keep repeating.
        else
            findDeps (Set.union found newNotAlreadyFound) newNotAlreadyFound

    // Find all bags that ultimately contain a shiny gold bag.
    findDeps Set.empty (set ["shiny gold"])
    |> Set.count
    |> printfn "Part 1 answer = %i"

    // Will return a list of topologically sorted bag types.
    // The tail of the list has the least number of precedents.
    let sortedRules =
        // Here we are doing a topological sort of the bag rules.
        // 'Sorted' has to be list (rather than a set) so that ordering is maintained.
        let rec sortRules (sorted: string list) (notSorted: Map<string, string Set>) =
            // If everything is sorted then we are done!
            if notSorted.IsEmpty then
                sorted

            // Otherwise...
            else
                // Find the next bag for which the precedents have already been sorted.
                let nextFound = 
                    notSorted
                    |> Map.toSeq
                    |> Seq.find (snd >> Set.forall (fun x -> List.contains x sorted))
                    |> fst

                // Now sort whatever has yet to be sorted.
                sortRules (nextFound::sorted) (notSorted |> Map.remove nextFound)

        sortRules [] ruleLinks    

    // Using the topologicaly sort above, determine the number of sub-bags that each bag type has.
    let bagCount =
        sortedRules
        // Start at the back where the bags have the least number of precedents.
        |> List.foldBack (
            fun bag (state: Map<string, int>) ->
                let currTotal =
                    rules.[bag]
                    |> List.sumBy (fun (count, subBag) -> count * (1 + state.[subBag]))

                state
                |> Map.add bag currTotal) <| Map.empty<string, int>
     
    bagCount.["shiny gold"]
    |> printfn "Part 2 answer = %i"

    0 // return an integer exit code
