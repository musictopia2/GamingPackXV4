namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.SavedGameClasses;
public class BasicSavedBoardDiceGameClass<P> : BasicSavedGameClass<P>, ISavedDiceList<SimpleDice>
    where P : class, IPlayerItem, new()
{
    public DiceList<SimpleDice> DiceList { get; set; } = [];
    public bool DiceVisible { get; set; }
}