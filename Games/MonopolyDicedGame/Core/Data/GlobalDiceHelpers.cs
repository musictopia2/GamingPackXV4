namespace MonopolyDicedGame.Core.Data;
public static class GlobalDiceHelpers
{
    public static BasicList<OwnedModel> OwnWhenRolling { get; set; } = [];
    public static BasicList<OwnedModel> OwnedOnBoard { get; set; } = [];
}