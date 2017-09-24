#load "../../references.fsx"
open NetMorpher.Analyser
open MBrace.FsPickler
open System
open System.Text.RegularExpressions
open System.IO
open System.Collections.Generic
open NetMorpher.Persistent
open NetMorpher.Structures
open NetMorpher.Structures.TrieMap

let morpher = NetMorpher.Load(DataFolder.BinaryLoader)

let lemmatize words = 
        words |> Array.map morpher.LemmatizeToken

let words = File.ReadAllLines <| DataFolder.GetPath "onegin.txt" 
                |> Seq.filter (fun w -> not <| Regex.IsMatch(w, @"\P{IsCyrillic}"))
                |> Seq.toArray

let megaWords = Array.concat [|words;words;words;words;words; words;words;words;words|] 

#time
lemmatize megaWords
#time


megaWords.Length