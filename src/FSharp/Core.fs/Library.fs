namespace TickTackTie.Core

open TMPro
open UnityEngine
open UnityEngine.UIElements

[<AllowNullLiteral>]
type Game() =
    inherit MonoBehaviour()
    
    [<DefaultValue>]
    [<SerializeField>]
    val mutable private field: Field

    [<DefaultValue>]
    [<SerializeField>]
    val mutable private restartButton: Button
    
    [<DefaultValue>]
    [<SerializeField>]
    val mutable private stateLabel: TextMeshProUGUI

and Field() =
    inherit MonoBehaviour()
    
    [<DefaultValue>]
    [<SerializeField>]
    val mutable private xSprite: Sprite
    
    [<DefaultValue>]
    [<SerializeField>]
    val mutable private oSprite: Sprite

    [<DefaultValue>]
    [<SerializeField>]    
    val mutable private cells: Cell[]

and [<RequireComponent(typedefof<Collider2D>)>]
    [<RequireComponent(typedefof<SpriteRenderer>)>]
    Cell() =
        inherit MonoBehaviour()
        
        [<DefaultValue>]
        [<SerializeField>]
        val mutable private field: Field
        
        [<DefaultValue>]
        [<SerializeField>]
        val mutable private collider: Collider2D
        
        [<DefaultValue>]
        [<SerializeField>]
        val mutable private renderer: SpriteRenderer
