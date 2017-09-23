namespace NetMorpher.Prediction

open NetMorpher.Types
open System
open NetMorpher.Structures.TrieMap
open StringUtils

module Predictor =
    let createLemmaHypothesis postfix (lowerToken:string) (x:TrieSearchResult<LemmaInfo list>) =
        let form = x.Prefix.Reverse()
        x.Record 
        |> Seq.map (fun r -> 
                    let record = r.GetFinal()
                    let replaced = form.Replace(postfix,"")
                    let replacer = if String.IsNullOrWhiteSpace replaced  
                                        then record.Lemma                                    
                                        else record.Lemma.Replace(replaced,"") 
                    let lemma = lowerToken.Replace(postfix, replacer)
                    LemmaInfo(0, lemma, record.Gr, 0)
                    )

    let MakePredictions (records:TrieSearchResult<LemmaInfo list>[]) (prefix:string) (lowerToken:string) =
        let postfix = prefix.Reverse()
        records |> Seq.collect (fun x-> createLemmaHypothesis postfix lowerToken x)
                |> Seq.toList

    let makePredictionsByFull (records:TrieSearchResult<LemmaInfo list>[]) (prefix:string) (lowerToken:string) =
        let postfix = prefix.Reverse()
        let appendix = lowerToken.Replace (postfix, "")
        records |> Seq.collect (fun x-> x.Record |> Seq.map ((fun l -> l.GetFinal()) >> (fun l -> LemmaInfo(0,appendix + l.Lemma, l.Gr,0))))
                |> Seq.toList