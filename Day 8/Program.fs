
open System
open System.IO

// These are the only op-codes we can accept.
type OpCode =
    | Nop
    | Acc
    | Jmp

// Represents the overall instruction for a given line.
type Instruction =
    | Instruction of OpCode * int

// This is the state that we track throughout execution.
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
        // Read in each line...
        File.ReadAllLines "Inputs.txt"
        // Split each line on the space.
        // eg. "Acc 20" -> [|"Acc"; "20"|]
        |> Array.map (fun x -> x.Split(' '))
        // Parse the op code and argument parts.
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

    // We need to be able to execute the instructions we've read in above.
    // The following function takes an array of instructions and the current state.
    // It then returns a new program state.
    // We have to supply all instructions in one go as, due to jumps, instructions are
    // not necessarily executed in order.
    let execute (instr_list: Instruction array) (state: State) _ =
        // Get the next instruction.
        let (Instruction (op, arg)) = instr_list.[state.position]

        // Return a new state.
        // This will depend on the next instruction to be run.
        // Indicators are also supplied which indicate whether a particular
        // line has already been run, and whether the program cursor
        // has been moved beyond the last instruction.
        let new_state =
            { (match op with
                | Nop -> { state with
                            position = state.position + 1 }
                | Acc -> { state with 
                            accumulator = state.accumulator + arg
                            position = state.position + 1 }
                | Jmp -> { state with
                            accumulator = state.accumulator
                            position = state.position + arg })
                with
                    lines_run =
                        Set.add state.position state.lines_run
                    duplicate_encountered =
                        Set.contains state.position state.lines_run }

        if new_state.position >= inputs.Length then
            { new_state with has_terminated = true }
        else
            new_state

    // The starting state for any program.
    let init_state =
        {
          accumulator = 0;
          position = 0;
          lines_run = Set.empty;
          duplicate_encountered = false
          has_terminated = false
        }

    // Keep executing instructions until a line is run for the second time.
    Seq.initInfinite id
    |> Seq.scan (execute inputs) init_state
    // Continue to execute whilst we've yet to run a line more than once.
    |> Seq.takeWhile (fun state -> not state.duplicate_encountered)
    // The last state provided will that just prior to a line being run a second time.
    |> Seq.last
    // Extract the accumulator.
    |> fun s -> s.accumulator
    |> printfn "Part 1 answer = %A"

    // Returns an executor where the instruction at 'idx' has been switched.
    // eg... Nop 20 -> Jmp 20   and   Jmp 20 -> Nop 20
    let execute_with_switch idx =
        // Duplicate the array.
        let new_inputs = Array.copy inputs

        // Switch the instruction at the specified line #.
        new_inputs.[idx] <-
            match new_inputs.[idx] with
            | Instruction (Nop, arg) -> Instruction (Jmp, arg)  // Switch with JMP
            | Instruction (Jmp, arg) -> Instruction (Nop, arg)  // Switch with NOP
            | Instruction (Acc, arg) -> Instruction (Acc, arg)  // Leave as is.

        // Return this as a partially applied executor.
        execute new_inputs

    // We want to find out which line, that when switched, yields a program that terminates properly.
    inputs
    |> Seq.indexed
    // We only care about the current line number.
    |> Seq.map fst
    // Keep switching out lines until we get a successful termination.
    |> Seq.pick (
        fun idx ->
            Seq.initInfinite id
            |> Seq.scan (execute_with_switch idx) init_state
            // Only ever execute until a line is about to be run for the second time.
            |> Seq.takeWhile (fun state -> not state.duplicate_encountered)
            // Can we find a point at which it has terminated?...
            |> Seq.tryFind (fun state -> state.has_terminated)
            // ...and at the correct position
            |> function
                | Some { has_terminated = true; position = pos; accumulator = acc }
                    when pos = inputs.Length
                        // Extract the accumulator at that point if so.
                        -> Some acc
                | _ -> None
       )
    |> printfn "Part 2 answer = %i"

    0 // return an integer exit code
