namespace TicTackToe.Core

open System
open TMPro
open UnityEngine
open UnityEngine.Events
open UnityEngine.UI
open TicTackToeDomain

type Update =
    | None
    | Action of Action

[<AllowNullLiteral>]
type GameComponent() =
    inherit MonoBehaviour()
    
    [<DefaultValue>]
    [<SerializeField>]
    val mutable private field: FieldComponent

    [<DefaultValue>]
    [<SerializeField>]
    val mutable private restartButton: Button
    
    [<DefaultValue>]
    [<SerializeField>]
    val mutable private stateLabel: TextMeshProUGUI
    
    let startEvent = Event<_>()
    let updateEvent = Event<Update>()
    
    member private this.Start() = startEvent.Trigger()
    member private this.Update() = updateEvent.Trigger(None)
    
    member private this.StartAsync() = Async.AwaitEvent startEvent.Publish
    member private this.UpdateAsync() = Async.AwaitEvent updateEvent.Publish
    
    member this.DoMoveAt position =
        updateEvent.Trigger (Action (Move position))
    
    member private this.Awake() =
        let gameFlow = async {
            do! this.StartAsync()
            
            this.restartButton.onClick.AddListener(UnityAction(
                this.OnRestartButtonClicked))
            
            let initialGame = init()
            
            let rec gameLoop currentGame = async {
                let! updateType = this.UpdateAsync()
                
                let game = match updateType with
                            | None -> currentGame
                            | Action action -> update currentGame action
                            
                this.UpdateView game
                
                return! gameLoop game
            }
            
            return! gameLoop initialGame
        }
        gameFlow |> Async.StartImmediate |> ignore

    member private this.UpdateView game =
        Array.iteri (fun index cell ->
            this.field.DrawCell (indexToPosition index) cell game.State)
            game.Board
            
        match game.State with
        | Playing -> this.stateLabel.text <- $"{game.Current.ToString()}'s turn"
        | GameOver result -> match result with
                             | Win player -> this.stateLabel.text <- $"{player.ToString()} wins!"
                             | Draw -> this.stateLabel.text <- "It's a draw!"
    
    member private this.OnRestartButtonClicked() =
        updateEvent.Trigger (Action Restart)
    
and FieldComponent() =
    inherit MonoBehaviour()
    
    [<DefaultValue>]
    [<SerializeField>]
    val mutable private xSprite: Sprite
    
    [<DefaultValue>]
    [<SerializeField>]
    val mutable private oSprite: Sprite

    [<DefaultValue>]
    [<SerializeField>]    
    val mutable private cells: CellComponent[]
    
    [<DefaultValue>]
    [<SerializeField>]    
    val mutable private game: GameComponent
    
    member this.DrawCell (x, y) cell gameState =
        let cellSprite = match cell with
                         | Empty -> null
                         | Occupied player -> match player with
                                              | X -> this.xSprite
                                              | O -> this.oSprite
        this.cells.[positionToIndex (x, y)].Draw cellSprite gameState
        
    member this.Draw cell =
        let index = this.cells |> Array.findIndex ((=) cell)
        let position = indexToPosition index
        this.game.DoMoveAt position

and [<RequireComponent(typedefof<Collider2D>)>]
    [<RequireComponent(typedefof<SpriteRenderer>)>]
    CellComponent() =
        inherit MonoBehaviour()
        
        [<DefaultValue>]
        [<SerializeField>]
        val mutable private field: FieldComponent
        
        [<DefaultValue>]
        [<SerializeField>]
        val mutable private collider: Collider2D
        
        [<DefaultValue>]
        [<SerializeField>]
        val mutable private renderer: SpriteRenderer
        
        member this.Draw cellSprite gameState =
            this.renderer.sprite <- cellSprite
            this.collider.enabled <- isNull cellSprite && gameState = Playing
            
        member private this.OnMouseDown() =
            this.field.Draw this
