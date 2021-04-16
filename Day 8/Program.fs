// Learn more about F# at http://fsharp.org

open System
open System.IO

type OpCode =
    | Nop
    | Acc
    | Jmp

type Instruction =
    | Instruction of OpCode * int

type State =
    {
        accumulator: int
        position: int
        lines_run: int Set
        duplicate_encountered: bool
        has_terminated: bool
    }


[<EntryPoint>]
let main argv =
    let inputs =
        File.ReadAllLines "Inputs.txt"
        |> Array.map (fun x -> x.Split(' '))
        |> Array.map (
            function
            | [|_op; _arg|] ->
                let arg = Int32.Parse(_arg)

                match _op with
                | "nop" -> Instruction (Nop, arg)
                | "acc" -> Instruction (Acc, arg)
                | "jmp" -> Instruction (Jmp, arg)
                | _ -> failwith "Unknown op-code."

            | _ -> failwith "Unable to parse line.")


    let execute instr_list (state: State) _ =        
        let (Instruction (op, arg)) = instr_list.[state.position]

        let duplicate_line_run = Set.contains state.position state.lines_run
        let new_lines_run = Set.add state.position state.lines_run

        let new_state =
            match op with
            | Nop -> { state with
                        position = state.position + 1
                        lines_run = new_lines_run
                        duplicate_encountered = duplicate_line_run }
            | Acc -> { state with 
                        accumulator = state.accumulator + arg
                        position = state.position + 1
                        lines_run = new_lines_run
                        duplicate_encountered = duplicate_line_run }
            | Jmp -> { state with
                        accumulator = state.accumulator
                        position = state.position + arg
                        lines_run = new_lines_run
                        duplicate_encountered = duplicate_line_run }

        if state.position >= inputs.Length then
            { new_state with has_terminated = true }
        else
            new_state

    let init_state =
        {
          accumulator = 0;
          position = 0;
          lines_run = Set.empty;
          duplicate_encountered = false
          has_terminated = false
        }

    let part1_run =
        let executor = execute inputs

        Seq.initInfinite id
        |> Seq.scan executor init_state
        |> Seq.takeWhile (fun state -> not state.duplicate_encountered)
        |> Seq.last

    part1_run.accumulator
    |> printfn "Part 1 answer = %A"

    let execute_with_switch idx =
        let new_inputs = Array.copy inputs

        new_inputs.[idx] <-
            match new_inputs.[idx] with
            | Instruction (Nop, arg) -> Instruction (Jmp, arg)
            | Instruction (Jmp, arg) -> Instruction (Nop, arg)
            | _ -> failwith "Unexpected error."

        execute new_inputs

    0 // return an integer exit code
