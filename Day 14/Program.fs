
open System
open System.IO
open System.Text.RegularExpressions

[<AutoOpen>]
module Instructions =

    let private reSetMask =
        Regex(@"^mask\s=\s([01X]{36})$")

    let private reSetMemory =
        Regex(@"^mem\[(\d+)\]\s=\s(\d+)$")

    let rec private genCombs (accrued: Int64 list) (idxs: Int64 Set) =
        seq {
            for item in idxs do
                match accrued with
                | acc::accs when item <= acc ->
                    yield! Seq.empty

                | _ ->
                    let newAccrued = item::accrued
                    yield (Set newAccrued)
                    yield! genCombs newAccrued (idxs |> Set.remove item)        
        }

    type Mask =
        | Mask of bool option array
        static member fromString (str: string) =
            str
            |> Seq.map (function | '0' -> Some false
                                 | '1' -> Some true
                                 | 'X' -> None
                                 | _ -> failwith "Unexpected.")
            |> Seq.toArray
            |> Mask

    type MemoryLocation =
        | MemoryLocation of Int64

    type MemoryValue =
        | MemoryValue of Int64

    type Memory =
        | Memory of Map<MemoryLocation, MemoryValue>

    type Masker =
        | Masker of (Memory -> MemoryLocation -> MemoryValue -> Memory)
        static member nullMasker = Masker (fun memory _ _ -> memory)

    type Instruction =
        | SetMask of Mask
        | UpdateMemory of location: MemoryLocation * value: MemoryValue

    type ComputerState =
        | State of masker: Masker * memory: Memory

    let private part1Masker (Mask mask) =
        let mask' =
            mask
            |> Array.rev
            |> Array.indexed

        let orPart =
            mask'
            |> Seq.sumBy (function | (idx, Some true) -> 1L <<< idx
                                   | _ -> 0L)

        let andPart =
            mask'
            |> Seq.sumBy (function | (_, Some false) -> 0L
                                   | (idx, _) -> 1L <<< idx)

        let masker (Memory memory) location (MemoryValue value) =
            let value' =
                (value ||| orPart) &&& andPart

            memory
            |> Map.add location (MemoryValue value')
            |> Memory

        Masker masker

    let private part2Masker (Mask mask) =
        let mask' =
            mask
            |> Array.rev
            |> Array.indexed

        let orPart =
            mask'
            |> Seq.sumBy (function | (idx, Some true) -> 1L <<< idx
                                   | _ -> 0L)

        let floatingOrParts =
            mask'
            |> Seq.choose (function | (idx, None) -> Some (1L <<< idx)
                                    | _ -> None)
            |> Set.ofSeq
            |> genCombs []
            |> Seq.map Seq.sum
            |> Seq.append [0L]

        let floatingAndBase =
            floatingOrParts |> Seq.max

        let floatingAndParts =
            floatingOrParts
            |> Seq.map (fun orMask -> ~~~ (floatingAndBase ^^^ orMask))

        let masker memory (MemoryLocation location) value =
            let applyMask (Memory memory) (floatingOrPart, floatingAndPart) =
                let location' = (location ||| orPart ||| floatingOrPart) &&& floatingAndPart

                memory
                |> Map.add (MemoryLocation location') value
                |> Memory

            Seq.zip floatingOrParts floatingAndParts
            |> Seq.fold applyMask memory

        Masker masker

    let private execute
            maskerFactory
            (State ((Masker masker) as currentMasker, ((Memory memory) as currentMemory))) =
        function
        | SetMask newMask ->
            State (maskerFactory newMask, currentMemory)

        | UpdateMemory (location, value) ->
            let newMemory =
                masker currentMemory location value

            State (currentMasker, newMemory)

    let part1Execute = execute part1Masker
    let part2Execute = execute part2Masker

    let ParseRow row =
        let maskMatch = reSetMask.Match row
        let memoryMatch = reSetMemory.Match row

        if maskMatch.Success then
            maskMatch.Groups.[1].Value
            |> Mask.fromString
            |> SetMask

        elif memoryMatch.Success then
            let location =
                int64 memoryMatch.Groups.[1].Value |> MemoryLocation

            let value =
                int64 memoryMatch.Groups.[2].Value |> MemoryValue

            UpdateMemory (location, value)

        else
            failwith "Unable to parse row."
     

[<EntryPoint>]
let main argv =
    let inputs =
        File.ReadAllLines "INPUTS.TXT"
        |> Array.map Instructions.ParseRow

    let initialState =
        State (Masker.nullMasker, Memory Map.empty)

    let sumMemoryItems (State (_, Memory memory)) =            
        memory
        |> Map.toSeq
        |> Seq.sumBy (function | (_, MemoryValue value) -> value)

    inputs
    |> Array.fold Instructions.part1Execute initialState
    |> sumMemoryItems
    |> printfn "Part 1 answer = %A"

    inputs
    |> Array.fold Instructions.part2Execute initialState
    |> sumMemoryItems
    |> printfn "Part 2 answer = %A"

    0
