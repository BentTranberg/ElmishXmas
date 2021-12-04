module DemoAsync.MainWindow

open System
open Elmish
open Elmish.WPF
open ElmishSupport

type ProgressIndicator = Idle | InProgress of percent:int

type Model =
    {
        StatusText: string
        ProgressIndicator: ProgressIndicator
    }

type Msg =
    | DoSomeWork of Aos<Result<int,string>>
    | RunWithProgress
    | UpdateStatus of statusText:string * progress:int
    | WorkIsComplete // This message could carry a result from the work done.

let initialModel =
    {
        StatusText = "Hello world!"
        ProgressIndicator = Idle
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
        { m with StatusText = statusText; ProgressIndicator = InProgress progress }, Cmd.none
    | WorkIsComplete ->
        { m with
            StatusText = "Work was completed."
            ProgressIndicator = Idle // This will enable the button when work has completed.
        }, Cmd.none
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
                dispatch (UpdateStatus ("Work complete", 100))
                dispatch WorkIsComplete
                }
            Async.StartImmediate delayedDispatch
        { m with
            StatusText = "Started progress."
            ProgressIndicator = InProgress 0 // This will disable the button when work starts.
        }, Cmd.ofSub incrementDelayedCmd
let bindings () : Binding<Model,Msg> list = [
    "StatusText" |> Binding.oneWay (fun m -> m.StatusText)
    "DoSomeWork" |> Binding.cmd (DoSomeWork (Started ()))
    "RunWithProgress" |> Binding.cmdIf (RunWithProgress,
        fun m -> match m.ProgressIndicator with Idle -> true | _ -> false)
    "Progress" |> Binding.oneWay (fun m ->
        match m.ProgressIndicator with Idle -> 0. | InProgress v -> float v)
    ]

let designTimeModel =
    {
        StatusText = "This is design time."
        ProgressIndicator = InProgress 30
    }

let designVm = ViewModel.designInstance designTimeModel (bindings ())
