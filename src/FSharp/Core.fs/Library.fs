namespace TicTackToe.Core

open TMPro
open UnityEngine
open UnityEngine.UIElements

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
