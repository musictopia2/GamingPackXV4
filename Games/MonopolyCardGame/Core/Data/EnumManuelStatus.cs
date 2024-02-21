namespace MonopolyCardGame.Core.Data;
public enum EnumManuelStatus
{
    None,
    InitiallyGoingOut,
    WentOutAfterDrawing5Cards,
    OtherPlayers //this means other players needs to manually figure out the cards.
}