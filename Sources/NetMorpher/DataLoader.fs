module DataLoader
open System.IO
open System.IO.Compression
open System.Collections.Generic
open NetMorpher.Structures.TrieMap
open DataStructures
open Utils
open System.Text

let loadCorpora path = 
    let lines (allLines: seq<string>) = 
        seq {
            let mutable builder = StringBuilder()
            let mutable needAppend = false
            for line in allLines do
                if (line.StartsWith "<text") then
                    builder <- builder.Append line
                    needAppend <- true
                else if (line.StartsWith "</text") then
                    builder <- builder.Append line
                    yield builder.ToString()
                    builder <- builder.Clear()
                    needAppend <- false
                else if (needAppend) then
                    builder <- builder.Append line
        }
        
    use file = File.OpenRead <| path
    use reader = new StreamReader(file)
    let sentences = lines <| readLines reader
                    |> Seq.map TextXml.Parse
                    |> Seq.collect (fun x -> x.Paragraphs |> Seq.collect (fun p -> p.Sentences))
                    |> Seq.toList
    sentences

let loadDict path = 
    use zip = ZipFile.OpenRead path
    let entry = zip.GetEntry "dict.opcorpora.xml"
    use stream = entry.Open()
    use reader = new StreamReader(stream)
    let fileLines = readLines reader
                    
    let gramemes =  fileLines
                    |> Seq.skipWhile (fun t -> not (t.TrimStart().StartsWith "<grammeme "))
                    |> Seq.takeWhile (fun t -> t.TrimStart().StartsWith "<grammeme ")
                    |> Seq.map (GrammemeXml.Parse >> (fun t -> (t.Name, t)))
                    |> dict
    
    let lemmas = fileLines 
                |> Seq.skipWhile (fun t -> not (t.TrimStart().StartsWith "<lemma "))
                |> Seq.takeWhile (fun t -> t.TrimStart().StartsWith "<lemma ")
                |> Seq.map (LemmaXml.Parse 
                    >> (fun lemma -> 
                        let gr = lemma.L.Gs |> Array.map (fun g -> gramemes.[g.V].Name)
                        let forms = lemma.Fs |> Array.map (fun f -> f.T) |> Array.distinct
                        (lemma.Id, (lemma.Id, lemma.L.T, gr, forms))
                        ))
                |> dict

    let types = fileLines 
                |> Seq.skipWhile (fun t -> not (t.TrimStart().StartsWith "<type "))
                |> Seq.takeWhile (fun t -> t.TrimStart().StartsWith "<type ")
                |> Seq.map (LinkTypeXml.Parse >> (fun t -> t.Id, t.Value))
                |> dict

    let links = fileLines 
                |> Seq.skipWhile (fun t -> not (t.TrimStart().StartsWith "<link "))
                |> Seq.takeWhile (fun t -> t.TrimStart().StartsWith "<link ")
                |> Seq.map (LemmaLinkXml.Parse >> (fun t -> t.To, t.From))
                |> dict

    let lemmas = lemmas.Values 
                    |> Seq.map (fun  (id, text, gr, forms) -> 
                                let found, element = links.TryGetValue id
                                let link: Option<int> = if found then Some element else None
                                (id, text, gr, forms, link))

    gramemes, lemmas, types


let loadWordForms path =
    let grammemes, lemmas, types = loadDict <| Path.Combine(path, @"dict.opcorpora.xml.zip")

    let corpora = loadCorpora <| Path.Combine(path, @"annot.opcorpora.no_ambig.xml") 
                    |> Seq.map (fun s -> s.Tokens |> Array.map(fun t -> (t.Tfr.V.L.Id, t.Text.Value.ToLowerInvariant())))
                    |> Seq.collect (fun s -> s)
                    |> Seq.groupBy (fun (t,_) -> t)
                    |> dict

    let lemmas = lemmas
                    |> Seq.map (fun (id, text, gr, forms, link) -> 
                                let found, lid = corpora.TryGetValue id
                                let count = if found then (lid |> Seq.length) else 0
                                (id, (LemmaInfo(id, text, gr, count), forms, link)))
                    |> dict

    for (info, _, link) in lemmas.Values do
        if link.IsSome then
            let lemma, _, _ = lemmas.[link.Value]
            info.Parent <- Some lemma
                

    let wordForms = lemmas.Values
                    |> Seq.collect (fun (lemma, forms, _) -> forms |> Seq.map (fun f -> (f.ToLowerInvariant() |> normalizeString, lemma)))
                    |> Seq.groupBy (fun (x, y) -> x)
            
    let map = new TrieMap<(LemmaInfo) list>()    
    let index = new Dictionary<int,LemmaInfo>()    
    for form, lemmas in wordForms do
        let formLemmas = (lemmas |> Seq.map (fun (x,y) -> y) |> Seq.sortByDescending (fun l -> l.Count) |> Seq.toList)
        map.Add (form |> reverseString, formLemmas)
    
    let index = lemmas.Values
                |> Seq.map ((fun (l,_,_) -> l) >> (fun l -> (l.Id, l)))
                |> dict

    (map, index)