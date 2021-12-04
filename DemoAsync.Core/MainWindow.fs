module DemoAsync.MainWindow

open System
open Elmish
open Elmish.WPF
open ElmishSupport

type Model =
    {
        StatusText: string
        Progress: int
    }

type Msg =
    | DoSomeWork of Aos<Result<int,string>>
    | RunWithProgress
    | UpdateStatus of statusText:string * progress:int
    | WorkIsComplete // This message could carry a result from the work done.

let initialModel =
    {
        StatusText = "Hello world!"
        Progress = 0
    }

let init () = initialModel, Cmd.none

let someWork () : Async<Result<int,string>> = async {
    do! Async.Sleep 3000
    return Result.Ok 7
    }

let private doSomeWork event (m: Model) =
    match event with
    | Started () ->
        { m with StatusText = "Starting." },
            Cmd.OfAsync.result (someWork () |> Async.map (Finished >> DoSomeWork))
    | Finished (Result.Ok value) ->
        { m with StatusText = $"Done, got {value}" }, Cmd.none
    | Finished (Result.Error error) ->
        { m with StatusText = $"Done, but with error {error}" }, Cmd.none

let update (msg: Msg) (m: Model) : Model * Cmd<Msg> =
    match msg with
    | DoSomeWork event -> doSomeWork event m
    | UpdateStatus (statusText, progress) ->
        { m with StatusText = statusText; Progress = progress }, Cmd.none
    | WorkIsComplete ->
        { m with StatusText = "Work was completed."; Progress = 0 }, Cmd.none
    | RunWithProgress ->
        let incrementDelayedCmd (dispatch: Msg -> unit) : unit =
            let delayedDispatch = async {
                do! Async.Sleep 1000
                dispatch (UpdateStatus ("Early work", 30))
                do! Async.Sleep 1000
                dispatch (UpdateStatus ("Still working", 60))
                do! Async.Sleep 1000
                dispatch (UpdateStatus ("Late work", 90))
                do! Async.Sleep 1000
                dispatch WorkIsComplete
                }
            Async.StartImmediate delayedDispatch
        { m with StatusText = "Started progress." }, Cmd.ofSub incrementDelayedCmd
let bindings () : Binding<Model,Msg> list = [
    "StatusText" |> Binding.oneWay (fun m -> m.StatusText)
    "DoSomeWork" |> Binding.cmd (DoSomeWork (Started ()))
    "RunWithProgress" |> Binding.cmd RunWithProgress
    "Progress" |> Binding.oneWay (fun m -> float m.Progress)
    ]

let designTimeModel = { StatusText = "This is design time."; Progress = 30 }

let designVm = ViewModel.designInstance designTimeModel (bindings ())
