namespace Mnd.Core

open Mnd.Core.Color
open Mnd.Core.Util
open System.Drawing

module Render =
    let render (f:int*int->RgbColor) (width:int, height:int) =
        let bmp = new Bitmap(width, height)
        let coords = seq {
            for i in 0..height-1 do
                for j in 0..width-1 do
                    yield (j,i)
        }

        let coordFunc = indexToCoord width

        let indexShader = coordFunc >> f >> RgbColor.map floatColorToScreen

        let pixels =
            Array.Parallel.init (width * height) indexShader
            |> Dithering.dither width

        let writeFunc idx color =
            let (x, y) = coordFunc idx
            bmp.SetPixel(x, y, (RgbColor.RawToGdiColor color))

        Array.iteri writeFunc pixels
        bmp
        
    let subSample cells i =
        let interval = 1.0 / (float cells)
        let halfInterval = interval/2.0
        seq { (i - 0.5 + halfInterval)..interval..(i + 0.5 - halfInterval) }
    
    let wrapAntiAliasing cells (f: float * float -> RgbColor) (x, y) =
        let samp = subSample cells

        let cols = seq {
            for dy in (samp y) do
                for dx in (samp x) do
                    yield f (dx, dy) }

        Seq.average cols

    let inverseProject (screenWidth, screenHeight) =
        let aspectRatio = screenWidth / screenHeight

        Vector2.mul (1.0/screenWidth, 1.0/screenHeight)
        >> Vector2.add (-0.5, -0.5)
        >> Vector2.mul (2.0, -2.0)
        >> Vector2.mul (aspectRatio, 1.0)

    let inverseView cameraPosition rotation scale =
        Vector2.scale scale >> Vector2.rotate rotation >> Vector2.add cameraPosition

