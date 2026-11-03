namespace MonopolyCardGame.Core.Data;
public enum EnumWhatStatus
{
    None,
    DrawOrTrade,
    Discard,
    TradeOnly,
    Either,
    LookOnly,
    EndTurn, //this means the player making trade can organize before ending turn.
    Other
    //ManuallyFigureOutMonopolies
}