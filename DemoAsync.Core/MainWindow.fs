module DemoAsync.MainWindow

open System
open Elmish
open Elmish.WPF
open ElmishSupport

type Model =
    {
        StatusText: string
    }

type Msg =
    | DoSomeWork of Aos<Result<int,string>>
    | RunWithProgress
    | UpdateStatusText of statusText:string

let initialModel =
    {
        StatusText = "Hello world!"
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
    | UpdateStatusText statusText ->
        { m with StatusText = statusText }, Cmd.none
    | RunWithProgress ->
        let incrementDelayedCmd (dispatch: Msg -> unit) : unit =
            let delayedDispatch = async {
                do! Async.Sleep 1000
                dispatch (UpdateStatusText "One")
                do! Async.Sleep 1000
                dispatch (UpdateStatusText "Two")
                do! Async.Sleep 1000
                dispatch (UpdateStatusText "Three")
                }
            Async.StartImmediate delayedDispatch
        { m with StatusText = "Started progress." }, Cmd.ofSub incrementDelayedCmd
let bindings () : Binding<Model,Msg> list = [
    "StatusText" |> Binding.oneWay (fun m -> m.StatusText)
    "DoSomeWork" |> Binding.cmd (DoSomeWork (Started ()))
    "RunWithProgress" |> Binding.cmd RunWithProgress
    ]

let designVm = ViewModel.designInstance initialModel (bindings ())
