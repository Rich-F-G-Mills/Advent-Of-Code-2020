// Learn more about F# at http://fsharp.org

open System
open System.IO

[<EntryPoint>]
let main argv =
    let inputs =
        File.ReadAllText "Inputs.txt"
        |> (fun x -> x.Split "\r\n\r\n")

    inputs
    |> Array.map (fun x -> x.Replace("\r\n",""))
    |> Array.map (Seq.distinct >> Seq.length)
    |> Array.sum
    |> printfn "Part 1 answer = %i"

    inputs
    |> Array.map (
        fun x ->
            x.Split("\r\n")
            |> Array.map Set.ofSeq
            |> Set.intersectMany
            |> Set.count)
    |> Array.sum
    |> printfn "Part 2 answer = %i"
    

    0 // return an integer exit code
