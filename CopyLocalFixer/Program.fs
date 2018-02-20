﻿// Learn more about F# at http://fsharp.org

open System
open System.Xml
open System.IO

type ReferenceState =
    | NoHintPath of XmlNode
    | Match of XmlNode
    | NoMatch of XmlNode

[<EntryPoint>]
let main argv =
    let path = argv.[0]
    let verbose = false
    let csprojs = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories)

    csprojs |> Array.iter (fun fileName ->

        
        let doc = XmlDocument()
        doc.Load (fileName)
        
        let nms = XmlNamespaceManager(doc.NameTable)
        nms.AddNamespace("foo", doc.DocumentElement.NamespaceURI)
        
        let outputPathNodes = doc.DocumentElement.SelectNodes ("//foo:PropertyGroup[@Condition]/foo:OutputPath", nms)
        let outputPathDebug = outputPathNodes.[0].InnerText
        let outputPathRelease = outputPathNodes.[1].InnerText

        if outputPathDebug = outputPathRelease then 
            let references = doc.DocumentElement.SelectNodes ("//foo:Reference", nms)
            references 
                |> Seq.cast<XmlNode> 
                |> Seq.map (fun r ->  
                    let hintPathNode = r.SelectSingleNode ("foo:HintPath", nms)
                    if hintPathNode = null then NoHintPath (r)
                    elif hintPathNode.InnerText.StartsWith outputPathDebug then Match(r)
                    else NoMatch (r))
                |> Seq.iter (fun referenceState ->
                    match referenceState with
                    | Match(r) ->
                        let assemblyNode = r.SelectSingleNode ("@Include", nms)
                        let node = doc.CreateNode(XmlNodeType.Element, "Private", doc.DocumentElement.NamespaceURI)
                        node.InnerText <- "True"
                        r.AppendChild(node) |> ignore
                        printfn "%s: set Private=True." assemblyNode.Value
                    | NoMatch(r) ->
                        if verbose then 
                            let assemblyNode = r.SelectSingleNode ("@Include", nms)
                            let hintPathNode = r.SelectSingleNode ("foo:HintPath", nms)
                            printfn "%s: No match for %s." assemblyNode.Value hintPathNode.InnerText
                    | NoHintPath(r) ->
                        if verbose then
                            let assemblyNode = r.SelectSingleNode ("@Include", nms)
                            printfn "%s: No hintpath." assemblyNode.Value
                )
        else 
            printfn "Output path %s is diffent from release path %s. Skpping..." outputPathDebug outputPathRelease
        
        doc.Save(fileName)


    )

    Console.ReadKey() |> ignore
    0 // return an integer exit code
