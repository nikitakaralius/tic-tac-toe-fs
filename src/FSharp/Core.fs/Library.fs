namespace FSharp

open UnityEngine

type FSharpLog() =
    inherit MonoBehaviour()
    member _.Start() = Debug.Log("Hello from F# class")