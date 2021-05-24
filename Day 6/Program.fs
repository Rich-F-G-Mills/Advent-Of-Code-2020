// Learn more about F# at http://fsharp.org

open System.IO

[<EntryPoint>]
let main argv =
    // Each array element corresponds to a passenger group.
    let inputs =
        File.ReadAllText "Inputs.txt"
        |> (fun x -> x.Split "\r\n\r\n")

    // Bring all answers within the group to the same line.
    // Then detemine number of unique characters within each group.
    inputs
    |> Seq.map (fun x -> x.Replace("\r\n",""))
    |> Seq.map (Seq.distinct >> Seq.length)
    |> Seq.sum
    |> printfn "Part 1 answer = %i"

    // Within each group, determine for which answers everyone answered 'yes' to.
    inputs
    |> Seq.map (
        fun x ->
            x.Split("\r\n")
            |> Seq.map Set.ofSeq
            |> Set.intersectMany
            |> Set.count)
    |> Seq.sum
    |> printfn "Part 2 answer = %i"

    0 // return an integer exit code
