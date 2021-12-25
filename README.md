# ElmishXmas
 
Demos of Elmish.WPF Xmas 2021

In the DemoAsync project, the AsyncOperationStatus type originally copied from
https://zaid-ajaj.github.io/the-elmish-book
has been renamed/abbreviated to Aos, and reworked in order to fit specifically
to work with Cmd.OfAsync.either

Cmd.OfAsync.either handles any exception not handled by the async function
being called. The idea with the Aos type is as follows

type Aos<'argument,'result,'error> =
    | StartAsync of 'argument
    | FinishAsync of 'result
    | FailAsync of 'error

* StartAsync takes an argument. This will frequently be unit, since usually the
handler for a specific message will extract the required data from the model.
In cases where the model does not contain the required data for the parameter
of the async function called with Cmd.OfAsync.either, StartAsync can supply
this data.

* FinishAsync will sometimes handle the Result type, and sometimes not. There
is sometimes a need to call functions that do not return Result.

* FailAsync will typically handle exceptions that results from calling an
async function with Cmd.OfAsync.either

The difference from the original AsyncOperationStatus is that Aos has the
additional case FailAsync, so that a normal result is handled by FinishAsync,
and any exception is handled by FailAsync.

It would also be possible to somehow inject exceptions into a Result's error
case in FinishAsync, but this would have some disadvantages;
  * it ties the FinishAsync case to the Result type, which then also put requirements on the
  signature of the async function being called, often demanding a wrapper function/lambda
  * not easy to distinguish exceptions that are not caught by the async
function from errors that are intentionally returned and delivered through FinishAsync
  * not that easy to just systematically route all or most FailAsync to a common handler,
  and also for that matter all or most FinishAsync Result.Error to a common handler.
