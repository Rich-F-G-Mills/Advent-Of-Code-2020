
open System.IO

[<EntryPoint>]
let main argv =
    // Read in the puzzle inputs.
    let inputs =
        File.ReadAllLines "Inputs.txt"
        |> Array.map int

    // Our device is rated for 3 jolts higher than the higest-rated adapter available.
    let deviceRating =
        inputs
        |> Array.max
        |> (+) 3

    // Finds the number of ways in which each grouping can be arranged.
    let rec findLayouts last_added remaining =
        match remaining with
        | [] -> 0L
        | [r] ->
            if r <= (last_added + 3) then 1L else 0L
        | r::rs ->
            if r <= (last_added + 3) then
                (findLayouts last_added rs) + (findLayouts r rs)
            else 0L                

    // Allocate contiguous sections of sorted joltage ratings into groups.
    // A new group is created if the difference between two adjacent ratings is 3 jolts.
    // This is will be an array of integer lists.
    let groupings =
        inputs
        |> Array.append [|0; deviceRating|]
        |> Array.sort
        |> Array.pairwise
        // Generate the difference between pairwise elements.
        |> Array.map (fun (x, y) -> (y, y - x))
        |> Array.rev
        // Allocate each of the ordered adapters into a group.
        |> Array.scan (fun (_, x) (y, z) -> (y, x + if z = 3 then 1 else 0)) (0, 0)
        |> Array.tail
        |> Array.rev
        |> Array.groupBy snd
        |> Array.map (snd >> (Array.map fst) >> Array.toList)

    // Get the highest rating within each group above.
    let priorCloses =
        groupings
        |> Array.map (List.last)
        |> Array.append [|0|]
        |> Array.take (Array.length groupings)

    inputs
    |> Array.append [|0; deviceRating|]
    |> Array.sortDescending
    |> Array.pairwise
    // Count the number 1 jolt and 3 jolt differences
    |> Array.countBy ((<||) (-))
    // Extract the counts for each jolt-difference.
    |> Array.map snd
    // Multiply these two numbers together.
    |> Array.reduce (*)
    |> printfn "Part 1 answer = %i"

    groupings
    |> Array.zip priorCloses
    |> Array.map ((<||) findLayouts)
    // Multiply together the number of arrangements for each sub-group.
    |> Array.reduce (*)
    |> printfn "Part 2 answer = %i"

    0
