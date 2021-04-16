// Learn more about F# at http://fsharp.org

open System
open System.IO

[<EntryPoint>]
let main argv =
    let inputs =
        File.ReadAllLines "Inputs.txt"

    let earliest_time = int64 inputs.[0]

    let bus_ids =
        inputs.[1].Split(',')
        |> Array.mapi (fun idx x -> (int64 idx, x |> System.Int64.TryParse |> function | (true, value) -> Some value | _ -> None))

    let bus_ids_part1 =
        bus_ids
        |> Array.choose snd

    bus_ids_part1
    |> Array.map(
        fun bus_id ->
            Seq.initInfinite int64
            |> Seq.map ((*) bus_id)
            |> Seq.skipWhile ((>) earliest_time)
            |> Seq.head)
    |> Array.zip bus_ids_part1
    |> Array.minBy snd
    |> fun (bus_id, time) -> bus_id * (time - earliest_time)
    |> printfn "Part 1 answer = %A"

    let bus_ids_part2 =
        bus_ids
        |> Array.choose (function | (idx, Some bus_id) -> Some (idx, bus_id) | _ -> None)

    // let bus_ids_part2 = [|(0L,1789L); (1L,37L); (2L, 47L); (3L, 1889L)|]

    // This can be done using https://www.youtube.com/watch?v=zIFehsBHB8o

    let part_2_inputs =
        bus_ids_part2
        |> Array.map (fun (idx, bus_id) -> ((bus_id - (idx % bus_id)) % bus_id, bus_id))

    let N =
        part_2_inputs
        |> Array.map snd
        |> Array.reduce (*)

    let N_i =
        part_2_inputs
        |> Array.map snd
        |> Array.map (fun x -> N / x)

    let x_i =
        part_2_inputs
        |> Array.map snd
        |> Array.zip N_i
        |> Array.map(
            fun (n_i, bus_id) ->
                Seq.initInfinite int64
                |> Seq.tail
                |> Seq.find (fun x -> (n_i * x) % bus_id = 1L)
                |> fun x_i -> x_i % bus_id)

    let chinese_remainder =
        part_2_inputs
        |> Array.map fst
        |> Array.zip3 N_i x_i
        |> Array.map(fun (n, x, b) -> b * n * x)
        |> Array.sum

    let part2_answer =
        chinese_remainder - N * (chinese_remainder / N)

    printfn "Part 2 answer = %i" part2_answer

    bus_ids_part2
    |> Array.forall (fun (idx, bus_id) -> (part2_answer + idx) % bus_id = 0L)
    |> printfn "All satisifed? %A"
    

    0 // return an integer exit code
