// Learn more about F# at http://fsharp.org

open System
open System.IO

[<EntryPoint>]
let main argv =
    let inputs =
        File.ReadAllLines "Inputs.txt"
        |> Array.map int64

    let part1_is_valid (target, preamble) =
        let rec find_pair remaining =
            match remaining with
            | [] -> None
            | p::ps ->
                if p >= target then
                    None
                else
                    match List.tryFind ((=) (target - p)) ps with
                    | Some p2 -> Some (p, p2)
                    | None -> find_pair ps
        
        match find_pair preamble with
        | Some _ -> true
        | None -> false

    let part1_answer =
        inputs
        |> Array.rev
        |> Array.windowed 26
        |> Array.map (fun x -> (x.[0], x.[1..] |> Array.sort |> Array.toList))
        |> Array.find (not << part1_is_valid)
        |> fst

    let rec find_series remaining =
        let found =
            remaining
            |> Seq.scan (fun (s, _) x -> (s + x, x)) (0L, 0L)
            |> Seq.skip 1
            |> Seq.takeWhile (fun (s, _) -> part1_answer >= s)
            |> Seq.toArray

        if (found |> Array.last |> fst) = part1_answer then
            found
            |> Array.map snd

        else
            find_series (Array.tail remaining)

    part1_answer
    |> printfn "Part 1 answer = %i"

    let part2_found =
        find_series inputs

    (Array.min part2_found) + (Array.max part2_found)
    |> printfn "Part 2 answer = %i"

    0 // return an integer exit code
