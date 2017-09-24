namespace NetMorpher.Prediction

open NetMorpher.Structures
open System
open NetMorpher.Structures.TrieMap
open StringUtils

module internal Predictor =
    let private createLemmaHypothesis postfix (lowerToken:string) (prefix:string) (records:LemmaStruct[]) =
        let form = prefix.Reverse()
        records 
        |> Seq.map (fun r -> 
                    let record = r.GetFinal()
                    let replaced = form.Replace(postfix,"")
                    let replacer = if String.IsNullOrWhiteSpace replaced  
                                        then record.Lemma                                    
                                        else record.Lemma.Replace(replaced,"") 
                    let lemma = lowerToken.Replace(postfix, replacer)
                    LemmaStruct(0,lemma, record.Gr, 0)
                    )

    let makePredictionsByFull (records:TrieSearchResult<LemmaStruct list>[]) (prefix:string) (lowerToken:string) =
        let postfix = prefix.Reverse()
        let appendix = lowerToken.Replace (postfix, "")
        records |> Seq.collect (fun x-> x.Record |> Seq.map ((fun l -> l.GetFinal()) >> (fun l -> LemmaStruct(0,appendix + l.Lemma, l.Gr, 0))))
                |> Seq.toList

    let MakePredictions (records:(string*LemmaStruct[])[]) (prefix:string) (lowerToken:string) =
        let postfix = prefix.Reverse()
        records |> Seq.collect (fun (prefix, records) -> createLemmaHypothesis postfix lowerToken prefix records)
                |> Seq.toList