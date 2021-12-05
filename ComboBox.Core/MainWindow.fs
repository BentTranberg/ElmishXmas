module ComboBox.MainWindow

open Elmish
open Elmish.WPF

let items = [ "First"; "Second"; "Third"; "Fourth" ]

type Model =
    {
        SelectedIndex: int
    }

type Msg =
    | SetSelectedIndex of int

let startModel =
    {
        SelectedIndex = 0
    }

let init () = startModel, Cmd.none

let update (msg: Msg) (m: Model) : Model * Cmd<Msg> =
    match msg with
    | SetSelectedIndex i -> { m with SelectedIndex = i }, Cmd.none

(*
let findSelectedIndexFromModel m =
    if (m.SelectedIndex >= 0) && (m.SelectedIndex < alternativeItems.Length) then
        let selectedValue = alternativeItems[m.selectedIndex]
        match alternativeItems |> List.tryFindIndex (fun x -> x = selectedValue) with
*)

let bindings () : Binding<Model,Msg> list = [
    "Items" |> Binding.oneWay (fun m -> items)
    "SelectedIndex" |> Binding.twoWay ((fun m -> m.SelectedIndex), SetSelectedIndex)
    ]

let designVm = ViewModel.designInstance startModel (bindings ())
