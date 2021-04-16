// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Text.RegularExpressions

module Instructions =

    let internal re_set_mask =
        Regex(@"^mask\s=\s([01X]{36})$")

    let internal re_set_memory =
        Regex(@"^mem\[(\d+)\]\s=\s(\d+)$")

    type Mask =
        | Mask of bool option array

    type Memory =
        | Memory of Map<int, bool array>

    type Instruction =
        | SetMask of Mask
        | UpdateMemory of location: int * value: bool array

    type ComputerState =
        | State of mask: Mask * memory: Memory

    let Execute
            (State (((Mask mask) as current_mask), ((Memory memory) as current_memory)))
            instruction =
        match instruction with
        | SetMask (Mask new_mask) -> State (Mask new_mask, current_memory)
        | UpdateMemory (location, value) ->
            let post_mask_value =
                Array.zip mask value
                |> Array.map (function | (Some x, _) -> x | (None, y) -> y)

            let new_memory =
                memory
                |> Map.add location post_mask_value

            State (current_mask, Memory new_memory)

    let ParseRow row =
        let mask_match = re_set_mask.Match row
        let memory_match = re_set_memory.Match row

        if mask_match.Success then
            let new_mask =
                mask_match.Groups.[1].Value
                |> Seq.map (
                    function
                    | '0' -> Some false
                    | '1' -> Some true
                    | 'X' -> None
                    | _ -> failwith "Unexpected masking character.")
                |> Seq.toArray

            SetMask (Mask new_mask)

        elif memory_match.Success then
            let mem_location =
                int memory_match.Groups.[1].Value

            let new_value =
                int memory_match.Groups.[2].Value

            let new_value_binary =
                Convert.ToString(new_value, 2).PadLeft(36, '0')
                |> Seq.map (function | '0' -> false | '1' -> true | _ -> failwith "Unexpected.")
                |> Seq.toArray

            UpdateMemory (mem_location, new_value_binary)

        else
            failwith "Unable to parse row."
     
open Instructions


[<EntryPoint>]
let main argv =
    let inputs =
        File.ReadAllLines "Inputs.txt"
        |> Array.map Instructions.ParseRow

    let initial_state =
        State (Mask Array.empty, Memory Map.empty)

    inputs
    |> Array.fold Instructions.Execute initial_state
    |> (function | State (_, Memory memory) -> memory)
    |> Map.toArray
    |> Array.map snd
    |> Array.map (Array.map (function | true -> '1' | false -> '0'))
    |> Array.map String.Concat
    |> Array.map (fun x -> Convert.ToInt64(x, 2))
    |> Array.sum
    |> printfn "Part 1 answer = %A"

    0 // return an integer exit code
