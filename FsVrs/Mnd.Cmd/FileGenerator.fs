namespace Mnd.Cmd

open System
open Mnd.Core
open Mnd.Core.Util
open Mnd.Core.Render

module FileGenerator =
    let generateFile fileName scale =
        let width = 2000
        let height = 1200
        let screenSize = (width, height)

//        let frameHeight = 0.00003
        let cameraPosition = (-0.998, 0.3011038462)
        //let frameHeight = 1.2
        //let cameraPosition = (-0.75, 0.0)
        let rotationAngle = 0.0
        let cameraScale = scale   // frameHeight

        let maxIterations = 1000
        let subsampleCells = 1
        let escapeRadius = 2000.0
        let cameraRotation = degToRad rotationAngle
        
        let inverseViewProj =
            inverseProject (toFloatTuple2 screenSize)
            >> inverseView cameraPosition cameraRotation cameraScale
            
        let coreShader = Shaders.cycleMandelbrotSmooth escapeRadius maxIterations
        let screenShader = inverseViewProj >> coreShader

        let aaShader = wrapAntiAliasing subsampleCells screenShader

        let finalShader =
            toFloatTuple2
            >> Vector2.add (0.5, 0.5)
            >> aaShader

        printfn "Rendering frame...scale=%A" scale
        let bmp = render finalShader screenSize
        bmp.Save(fileName)
        1
        
    let seqMult (n: int) s = Math.Pow(s, 1.0 / float n)
    
    let getFileSeq max endScale =
        let mult = seqMult max endScale
        [0..max] |> List.map (fun n -> (n, Math.Pow(mult, n))) 
        
    let generateSeq =
        let endScale = 0.00003
        let count = 5;
        let fileIndexes = getFileSeq count endScale
            
        let files = fileIndexes |> List.map (fun (n, scale) -> (string (sprintf "D:\\Temp\\mandelbrot\\file%06i.png" n), scale))
        for (fileName, scale) in files do
            generateFile fileName scale |> ignore
        1