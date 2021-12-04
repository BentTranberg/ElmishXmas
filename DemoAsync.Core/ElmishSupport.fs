module DemoAsync.ElmishSupport

module Async =

    let map f computation = async.Bind(computation, f >> async.Return)

// The Deferred stuff from
// https://zaid-ajaj.github.io/the-elmish-book
// Modified to also support a start parameter,
// for cases when input data is not found in the model.

type AsyncOperationStatusWithParameter<'a,'r> = Started of 'a | Finished of 'r
type AosParam<'a,'r> = AsyncOperationStatusWithParameter<'a,'r> // Abbreviation

type AsyncOperationStatus<'r> = AsyncOperationStatusWithParameter<unit,'r>
type Aos<'r> = AsyncOperationStatus<'r> // Abbreviation

type Deferred<'t> =
    | HasNotStartedYet
    | InProgress
    | Resolved of 't
