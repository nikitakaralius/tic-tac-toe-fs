module TicTacToe.Core.Domain

type Position = int * int

type Action =
    | Move of Position
    | Restart

type Update =
    | None
    | Action of Action
    
type Player =
    | X
    | O
    
type GameOverResult =
    | Win of Player
    | Draw
    
type Cell =
    | Empty
    | Occupied of Player
    
type GameState =
    | Playing
    | GameOver of GameOverResult
    
type Game = {
    State: GameState
    Current: Player
    Board: Cell[]
}

let fieldWidth = 3
let fieldHeight = 3

let offsets = seq [
    (-1,  0)
    (-1, -1)
    ( 0, -1)
    (-1,  1)
]

let indexToPosition index =
    let x = index % fieldWidth
    let y = index / fieldHeight
    (x, y)
    
let positionToIndex (x, y) =
    x + y * fieldHeight
    
let init() =
    {
        State = Playing
        Current = X
        Board = Array.create (fieldWidth * fieldHeight) Empty
    }
    
let correct x y =
    x >= 0 && x < fieldWidth
    && y >= 0 && y < fieldHeight
    
let wins currentPlayer (cells: Cell[]) (playerX, playerY) =
    let rec search x y offsetX offsetY =
        if correct x y then
            let position = (x, y)
            
            seq {
                match cells.[positionToIndex position] with
                | Occupied player when player = currentPlayer ->
                    yield position
                    yield! (search (x + offsetX) (y + offsetY) offsetX offsetY)
                | _ -> ()
            }
        else
            Seq.empty
    
    offsets
    |> Seq.map (fun (offsetX, offsetY) -> seq {
        yield! search playerX playerY offsetX offsetY
        yield! (search (playerX - offsetX) (playerY - offsetY) -offsetX -offsetY)})
    |> Seq.exists ((Seq.tryItem 2) >> Option.isSome)
    
let draw (cells: Cell[]) =
    cells
    |> Array.exists (function | Empty -> true | _ -> false)
    |> (=) false
    
let nextPlayerAfter currentPlayer =
    match currentPlayer with
    | X -> O
    | O -> X
    
let update game action =
    match action with
    | Restart ->
        init()
    | Move (px, py) ->
        let board = Array.mapi (fun index cell ->
            let cx, cy = indexToPosition index
            if cx = px && cy = py then
               Occupied(game.Current) else cell) game.Board
        
        let currentPlayer = game.Current
        match wins currentPlayer board (px, py) with
        | true -> {game with Board = board; State = GameOver (Win currentPlayer)}
        | _ -> match draw board with
                | true -> {game with Board = board; State = GameOver Draw}
                | _ -> {game with Board = board; Current = nextPlayerAfter currentPlayer}
