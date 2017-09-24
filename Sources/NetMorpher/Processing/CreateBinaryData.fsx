#load "../../../references.fsx"
#load "OdictDataLoader.fsx"
#load "MapBuilder.fsx"

open System.IO
open System.IO.Compression
open NetMorpher.Persistent
open MBrace.FsPickler

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


let path = DataFolder.GetPath @"dict.opcorpora.xml.zip"
let odictData = ZipModule.readZip path "dict.opcorpora.xml" OdictDataLoader.readOdictFile
let map, grammemes, lemmas = MapBuilder.BuildMap odictData
let binarySerializer = FsPickler.CreateBinarySerializer()
using (File.OpenWrite @"E:\Projects\TextProcessing\NetMorpher\Data\map.bin")
    (fun stream -> binarySerializer.SerializeSequence(stream, map.GetChilds()) |> ignore)

using (File.OpenWrite @"E:\Projects\TextProcessing\NetMorpher\Data\lemmas.bin")
    (fun stream -> binarySerializer.SerializeSequence(stream, lemmas)|> ignore)

using (File.OpenWrite @"E:\Projects\TextProcessing\NetMorpher\Data\grammemes.bin")
    (fun stream -> binarySerializer.SerializeSequence(stream, grammemes))