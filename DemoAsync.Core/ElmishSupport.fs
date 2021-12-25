module DemoAsync.ElmishSupport

module Async =

    let map f computation = async.Bind(computation, f >> async.Return)

// The Deferred type and a modified AsyncOperationStatus type from
// https://zaid-ajaj.github.io/the-elmish-book
// AsyncOperationStatus is modified to support a start parameter,
// for cases when input data is not found in the model. It also
// has a FinishAsync case that often handles the Result type,
// and a FailAsync case that usually handles the exception from
// Cmd.Async.either

type Aos<'argument,'result,'error> = // short for AsyncOperationStatus
    | StartAsync of 'argument
    | FinishAsync of 'result
    | FailAsync of 'error

type Deferred<'t> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 't
