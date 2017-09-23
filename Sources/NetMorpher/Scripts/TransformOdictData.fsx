#load "../../../references.fsx"
open System.IO
open System.IO.Compression
open FSharp.Data
open MBrace.FsPickler
open NetMorpher.DataTypes
open NetMorpher.Persistent


module XmlTypes =
    type Grammeme = XmlProvider<"""<grammeme parent="POST"> <name>NOUN</name> <alias>СУЩ</alias> <description>имя существительное</description> </grammeme>""">
    type LinkType = XmlProvider<"""<type id="2">ADJF-COMP</type>""">
    type Lemma = XmlProvider<"""<lemma id="1" rev="1"> <l t="ёж"> <g v="NOUN" /> <g v="anim" /> <g v="masc" /> </l> <f t="ёж"> <g v="sing" /> <g v="nomn" /> </f> <f t="ежа"> <g v="sing" /> <g v="gent" /> </f> </lemma>""">
    type LemmaLink = XmlProvider<"""<link id="1" from="5" to="6" type="1"/>""">


module ZipModule =
    let rec private readLines (reader:StreamReader) = 
        seq {
            let rawLine = reader.ReadLine()
            match rawLine with 
                | null -> ()
                | _ -> 
                    yield rawLine
                    yield! readLines reader
        }

    let readZip zipFilePath entryFileName readFunction =
        use zip = ZipFile.OpenRead zipFilePath
        let entry = zip.GetEntry entryFileName
        use stream = entry.Open()
        use reader = new StreamReader(stream)
        let lines = reader |> readLines
        lines |> readFunction



let readOdictFile (fileLines:seq<string>) =
    let mutable gramemes, lemmas, types, links = [], [], [], []
    fileLines 
    |> Seq.iter (fun line -> 
        match line.TrimStart() with
            | x when x.StartsWith "<grammeme " ->   (gramemes   <- (XmlTypes.Grammeme.Parse x)::gramemes)
            | x when x.StartsWith "<type " ->       (types      <- (XmlTypes.LinkType.Parse x)::types)
            | x when x.StartsWith "<link " ->       (links      <- (XmlTypes.LemmaLink.Parse x)::links)
            | x when x.StartsWith "<lemma " ->      (lemmas     <- (XmlTypes.Lemma.Parse x)::lemmas)
            | _ -> ()
    )

    let gramemes = gramemes |> List.map (fun t -> (t.Name, (t.Name, t.Alias, t.Description, t.Parent))) |> dict
    let types = types |> List.map (fun t -> (t.Id, t.Value))
    let links = links |> List.map (fun t -> t.To, t.From) |> dict
    let lemmas = lemmas 
                      |> List.map (fun lemma -> 
                                             let gr = lemma.L.Gs |> Array.map (fun g -> g.V) |> List.ofArray
                                             let forms = lemma.Fs |> Array.map (fun f -> f.T) |> Array.distinct |> List.ofArray
                                             let found, element = links.TryGetValue lemma.Id
                                             let link: Option<int> = if found then Some element else None
                                             (lemma.Id, lemma.L.T, gr, forms, link))

    gramemes.Values, lemmas, types


let path = DataFolder.GetPath @"dict.opcorpora.xml.zip"

let gramemes, lemmas, types = ZipModule.readZip path "dict.opcorpora.xml" readOdictFile

let binarySerializer = FsPickler.CreateBinarySerializer()


using (File.OpenWrite <| DataFolder.Data.lemmas)
    (fun stream -> binarySerializer.SerializeSequence<LemmasTuple>(stream, lemmas))

using (File.OpenWrite <| DataFolder.Data.grammemes)
    (fun stream -> binarySerializer.SerializeSequence<GramemesTuple>(stream, gramemes))

using (File.OpenWrite <| DataFolder.Data.types)
    (fun stream -> binarySerializer.SerializeSequence<LemmaLinkTypeTuple>(stream, types))
