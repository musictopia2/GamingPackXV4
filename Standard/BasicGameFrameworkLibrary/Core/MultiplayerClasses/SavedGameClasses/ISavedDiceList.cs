namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.SavedGameClasses;
public interface ISavedDiceList<D>
    where D : IStandardDice, new()
{
    DiceList<D> DiceList { get; set; }
}