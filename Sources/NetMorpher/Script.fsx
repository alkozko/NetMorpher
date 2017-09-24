#load "../../references.fsx"
open NetMorpher.Analyser
open MBrace.FsPickler
open System.IO
open System.Collections.Generic
open NetMorpher.Persistent
open NetMorpher.Structures
open NetMorpher.Structures.TrieMap

let morpher = NetMorpher.Load(DataFolder.BinaryLoader)

printf "%s" <| morpher.LemmatizeToken "хомякового"