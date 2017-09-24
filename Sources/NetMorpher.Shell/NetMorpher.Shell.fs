module NetMorpher.Shell

open NetMorpher.Analyser
open NetMorpher.Structures
open System.IO
open System

let tokenize (line: string) =
    line.Split(' ',',','.','?','!')

let mapPOS pos = 
    match pos with
        | "NOUN" -> "S"
        | "ADJF" -> "A"
        | "ADJS" -> "A"
        | "COMP" -> "A"
        | "VERB" -> "V"
        | "INFN" -> "V"
        | "PRTF" -> "V"
        | "PRTS" -> "V"
        | "GRND" -> "V"
        | "PREP" -> "PR"
        | "CONJ" -> "CONJ"
        | "NUMR" -> "ADV"
        | "ADVB" -> "ADV"
        | "NPRO" -> "ADV"
        | "PRED" -> "ADV"
        | "PRCL" -> "ADV"
        | "INTJ" -> "ADV"
        | _ -> "ADV"

let format token (lemmas:ILemmaInfo) =
    String.Format("{0}{{{1}={2}}}",token, lemmas.Lemma, mapPOS <| lemmas.Gr.[0])

let result (morpher: NetMorpher) (token: string)  =
    let res = morpher.AnnalyzeToken token
    format token res.Head

let equals x o =
    o.Equals(x)

let lemmatizeLine morpher line = 
    tokenize line
    |> Seq.filter (equals String.Empty >> not)
    |> Seq.toList
    |> List.map (fun t -> t, result morpher t) 


[<EntryPoint>]
let main argv =
    let morpher = NetMorpher.Load(NetMorpher.Persistent.DataFolder.BinaryLoader)
    GC.Collect()
    printfn "%i" <| (GC.GetTotalMemory(true) / (1024L*1024L))
    GC.Collect()
    printfn "%i" <| (GC.GetTotalMemory(true) / (1024L*1024L))

    while (true) do
        try
            let line = Console.ReadLine()
            let tokens = lemmatizeLine morpher line

            Console.WriteLine(String.Join(" ", tokens))
        with
            _ -> ()
    0 // return an integer exit code
