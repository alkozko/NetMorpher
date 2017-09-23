namespace NetMorpher.Types

open System
open FSharp.Data

type LemmaInfo(id:int, lemma:string, gramemma:string[], count:int) =
    member this.Lemma = lemma
    member this.Gr = gramemma
    member this.Id = id
    member this.Count = count
    member val Parent:LemmaInfo option = None with get, set

    member this.GetFinal() = 
        match this.Parent with
            | Some x -> x.GetFinal()
            | None -> this

    override this.ToString() =
        lemma + "{" + String.Join(",", gramemma) + "}"