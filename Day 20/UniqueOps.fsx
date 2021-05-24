

type Transformation<'a> =
    {
        Invoke: 'a [,] -> 'a [,]
        Name: string
    }
        
let inline (|>>) lhs rhs =
    {
        Invoke = lhs.Invoke >> rhs.Invoke
        Name = $"{lhs.Name} -> {rhs.Name}"            
    }

let rotate =
    {
        Invoke =
            fun grid ->
                let (height, width) = (Array2D.length1 grid, Array2D.length2 grid)

                let _rotate row col =
                    grid.[height - col - 1, row]

                Array2D.init width height _rotate

        Name = "Rotate"
    }

let flip_horiz =
    {
        Invoke =
            fun grid ->
                let (height, width) = (Array2D.length1 grid, Array2D.length2 grid)

                let _flip row col =
                    grid.[row, width - col - 1]

                Array2D.init height width _flip

        Name = "Flip_Horiz"
    }

let start_grid =
    Array2D.init 3 3 (fun row col -> col + row * 3)

let ops = [rotate; flip_horiz] |> List.toSeq

let rec generate_combs accrued =
    seq {
        for a in accrued do
        for op in ops do
        yield a |>> op
    }

Seq.initInfinite id
|> Seq.take 8
|> Seq.scan (fun accrued _ -> generate_combs accrued) ops
|> Seq.concat
|> Seq.map (fun t -> (t, t.Invoke start_grid))
|> Seq.filter (fun t -> (snd t) <> start_grid)
|> Seq.distinctBy snd
|> Seq.map (fun t -> (fst t).Name)
|> Seq.iter (printfn "%s")