module DataStructures

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
        lemma + "{" + String.Join(",",gramemma) + "}"



type GrammemeXml = XmlProvider<"""<grammeme parent="POST"> <name>NOUN</name> <alias>СУЩ</alias> <description>имя существительное</description> </grammeme>""">
type LemmaXml = XmlProvider<"""<lemma id="1" rev="1"> <l t="ёж"> <g v="NOUN" /> <g v="anim" /> <g v="masc" /> </l> <f t="ёж"> <g v="sing" /> <g v="nomn" /> </f> <f t="ежа"> <g v="sing" /> <g v="gent" /> </f> </lemma>""">
type LinkTypeXml = XmlProvider<"""<type id="2">ADJF-COMP</type>""">
type LemmaLinkXml = XmlProvider<"""<link id="1" from="5" to="6" type="1"/>""">
type TextXml = XmlProvider<"_annot.opcorpora.no_ambig_sample.xml">
