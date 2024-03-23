// For more information see https://aka.ms/fsharp-console-apps

open Mnd.Core

printfn "Mandelbrot Generator"

open Mnd.Core.Util
open Mnd.Core.Render

let generateSingleFile =
    let fname = @"D:\Temp\mandelbrot\out.png"
    let width = 2000
    let height = 1200
    let screenSize = (width, height)

    let frameHeight = 0.00003
    let cameraPosition = (-0.998, 0.3011038462)
    // let frameHeight = 1.2
    // let cameraPosition = (-0.75, 0.0)
    let rotationAngle = 0.0
    let cameraScale = frameHeight   // The same?

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

    printfn "Rendering frame..."
    let stopWatch = System.Diagnostics.Stopwatch.StartNew()
    let bmp = render finalShader screenSize
    stopWatch.Stop()
    printfn "Render complete."
    printfn "Elapsed time: %f seconds." stopWatch.Elapsed.TotalSeconds

    printfn "Saving to: %s" fname
    bmp.Save(fname)
    printfn "Render saved."

[<EntryPoint>]
let main args =
    // Mnd.Cmd.FileGenerator.generateSeq |> ignore
    generateSingleFile |> ignore
    printfn "Finish."
    0