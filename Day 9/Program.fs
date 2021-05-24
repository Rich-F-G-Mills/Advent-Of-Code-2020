// Learn more about F# at http://fsharp.org

open System.IO

[<EntryPoint>]
let main argv =
    // Read in the puzzle inputs and convert into 64bit integers.
    let inputs =
        File.ReadAllLines "Inputs.txt"
        |> Seq.map int64
        |> Seq.toList

    let part1IsValid (target, preamble) =
        // Try and find a pair of numbers within the preamble that sum to the target.
        // Note that this method assumes that the supplied preamble is in ascending order.
        let rec findPair remaining =
            match remaining with
            // If there is no numbers left to check then we cannot continue.
            | [] -> false
            | p::ps ->
                // If the first number is greater than the target then no point continuing further.
                if p >= target then
                    false
                else
                    ps
                    |> Seq.skipWhile ((>) (target - p))
                    |> Seq.tryHead
                    |> function
                        | Some x when p + x = target -> true
                        | _ -> findPair ps
        
        findPair preamble

    let part1Answer =
        inputs
        |> List.rev
        |> List.windowed 26
        |> Seq.map (fun x -> (x |> List.head, x |> List.tail |> List.sort))
        |> Seq.find (not << part1IsValid)
        |> fst

    part1Answer
    |> printfn "Part 1 answer = %i"

    // Here we try to find a contiguous series of numbers that sum to the part 1 answer above.
    let rec findSeries remaining =
        // This is similar to a cumulative sum however we are also storing the individual numbers making up the sum as well.
        let found =
            remaining
            |> Seq.scan (+) 0L
            |> Seq.skip 1
            |> Seq.zip remaining
            |> Seq.takeWhile (snd >> ((>=) part1Answer))
            |> Seq.toArray

        // Check to see if the cumulative sum is equal to the part 1 answer above.
        found
        |> Array.tryLast
        |> function
            | Some (_, sum) when sum = part1Answer ->
                found
                |> Array.map fst
            | _ -> findSeries (List.tail remaining)

    // Store the contiguous list of numbers making up the sum.
    let part2Found =
        findSeries inputs

    Array.min part2Found 
    |> (+) (Array.max part2Found)
    |> printfn "Part 2 answer = %i"

    0
