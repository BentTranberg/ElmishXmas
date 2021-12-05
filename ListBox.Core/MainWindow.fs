module ListBox.MainWindow

open System
open Elmish
open Elmish.WPF

type Guid with
    static member newGuid with get() = Guid.NewGuid()

type ItemId = private ItemId of Guid with
    static member crack (ItemId v) = v
    member x.q = ItemId.crack x
    static member create id = ItemId id

type ItemModel =
    {
        Id: ItemId
        Name: string
        IsSelected: bool
    }
    static member create id name sel =
        { ItemModel.Id = ItemId.create id; Name = name; IsSelected = sel }

type Model =
    {
        Items: ItemModel List
        SelectedItemId: ItemId option
    }

type Msg =
    | SetSelectedItemId of ItemId option
    | SetIsSelected of ItemId * bool

let initialModel =
    let selectedItemGuid = Guid.newGuid
    {
        Items = [
            ItemModel.create Guid.newGuid "One" false
            ItemModel.create Guid.newGuid "Two" false
            ItemModel.create Guid.newGuid "Three" true
            ItemModel.create Guid.newGuid "Four" true
            ItemModel.create Guid.newGuid "Five" false
            ItemModel.create Guid.newGuid "Six" false
            ItemModel.create selectedItemGuid "Seven" true
            ItemModel.create Guid.newGuid "Eight" true
            ItemModel.create Guid.newGuid "Nine" true
            ItemModel.create Guid.newGuid "Ten" false
            ItemModel.create Guid.newGuid "Eleven" false
            ]
        SelectedItemId = selectedItemGuid |> ItemId.create |> Some
    }

let init () = initialModel, Cmd.none

let update (msg: Msg) (m: Model) : Model * Cmd<Msg> =
    match msg with
    | SetSelectedItemId itemId -> { m with SelectedItemId = itemId }, Cmd.none
    | SetIsSelected (id, isSelected) ->
        let items =
            m.Items
            |> List.map (fun e -> if e.Id = id then { e with IsSelected = isSelected } else e)
        { m with Items = items }, Cmd.none

let itemBindings () = [
    "Id" |> Binding.oneWay (fun (m, item) -> item.Id.q)
    "Name" |> Binding.oneWay (fun (m, item) ->
        let tailInfo =
            if m.SelectedItemId.IsSome && m.SelectedItemId.Value = item.Id then " [[SELECTED]]"
            elif item.IsSelected then " <<selected>>"
            else ""
        item.Name + tailInfo)
    "IsSelected" |> Binding.twoWay (
        (fun (_, item) -> item.IsSelected),
        (fun isSelected (_, item) -> SetIsSelected (item.Id, isSelected)))
    ]

let bindings () : Binding<Model,Msg> list = [
    "Items" |> Binding.subModelSeq ((fun m -> m.Items), (fun item -> item.Id), itemBindings)
    "SelectedItemId" |> Binding.twoWay (
        (fun m -> (match m.SelectedItemId with Some x -> box x.q | None -> null)),
        (fun (v: obj) ->
            match v with
            | :? Guid as v -> SetSelectedItemId (Some (ItemId.create v))
            | _ -> SetSelectedItemId None))
    "SelectedItemIdInfo" |> Binding.oneWay (fun m ->
        match m.SelectedItemId with
        | Some itemId ->
            let s = itemId.q.ToString("D").ToUpperInvariant()
            $"Selected item has id %s{s}"
        | None -> "No item is selected.")
    ]

let designVm = ViewModel.designInstance initialModel (bindings ())
