open System
open System.Xml
open System.IO

type ReferenceState =
    | NoHintPath of XmlNode
    | Match of XmlNode
    | NoMatch of XmlNode

let addSlash (dir: string) = 
    if not (dir.EndsWith("\\")) then dir + "\\" else dir

let hasSamePath outputPath (hintPath: string) assemblyName =
    let cutOff =  hintPath.LastIndexOf('\\')
    if(cutOff > -1) then 
        let assemblyPath = hintPath.Substring (0, cutOff)
        let p1 = (addSlash outputPath)
        let p2 = (addSlash assemblyPath)
        p1 = p2
    else false

let fixCsproj fileName 
              (outputPathNodes: XmlNodeList) 
              (doc: XmlDocument) 
              (nms: XmlNamespaceManager) 
              verbose =

    let outputPathDebug = outputPathNodes.[0].InnerText
    let outputPathRelease = outputPathNodes.[1].InnerText

    let getAssemblyName (node: XmlNode) =
        let assemblyNode = node.SelectSingleNode ("@Include", nms)
        let text = assemblyNode.InnerText
        let cutOff = text.IndexOf(',')

        if cutOff > -1 then text.Substring(0, cutOff) 
        else text
 
    let handleMatch (referenceNode: XmlNode) =
        let privateNode = referenceNode.SelectSingleNode ("foo:Private", nms)
        if privateNode = null then
            let assemblyNode = referenceNode.SelectSingleNode ("@Include", nms)
            let node = doc.CreateNode(XmlNodeType.Element, "Private", doc.DocumentElement.NamespaceURI)
            node.InnerText <- "False"
            referenceNode.AppendChild(node) |> ignore
            printfn "%s: set Private to False." assemblyNode.Value
        else 
            privateNode.InnerText <- "False"
            printfn "%s: Private was 'True'. Set to 'False'." (getAssemblyName referenceNode)

    let handleNoMatch (referenceNode: XmlNode) =
        let privateNode = referenceNode.SelectSingleNode ("foo:Private", nms)
        if privateNode <> null then
            privateNode.InnerText <- "True"
                        
            let assemblyName = getAssemblyName referenceNode
            let hintPathNode = referenceNode.SelectSingleNode ("foo:HintPath", nms)
            printfn "%s: Has match for %s even thought path doesn't match. Private set True." assemblyName hintPathNode.InnerText
        else
            let hintPathNode = referenceNode.SelectSingleNode ("foo:HintPath", nms)
            let assemblyName = getAssemblyName referenceNode
            printfn "%s: No match for %s." assemblyName hintPathNode.InnerText

    let handleNoHintPath (referenceNode: XmlNode) =
        if verbose then
            let assemblyName = getAssemblyName referenceNode
            printfn "%s: No hintpath." assemblyName

    if outputPathDebug = outputPathRelease then 
        let references = doc.DocumentElement.SelectNodes ("//foo:Reference", nms)
        references 
            |> Seq.cast<XmlNode> 
            |> Seq.map (fun r ->  
                let hintPathNode = r.SelectSingleNode ("foo:HintPath", nms)
                let assemblyName = getAssemblyName r
                if hintPathNode = null then NoHintPath (r)
                elif hasSamePath outputPathDebug hintPathNode.InnerText assemblyName  then Match(r)
                else NoMatch (r))
            |> Seq.iter (fun referenceState ->
                match referenceState with
                | Match(r) -> handleMatch r
                | NoMatch(r) -> handleNoMatch r
                | NoHintPath(r) -> handleNoHintPath r
            )
    else 
        printfn "Warning -- %s: Output path %s is diffent from release path %s. Skpping..." fileName outputPathDebug outputPathRelease
        

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

        if outputPathNodes.Count > 0 then
            fixCsproj fileName outputPathNodes doc nms verbose
            doc.Save(fileName)
    )

    0 // return an integer exit code
