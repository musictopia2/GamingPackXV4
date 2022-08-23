namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.SavedGameClasses;

public class BasicSavedCardClass<P, D> : BasicSavedGameClass<P>
    where P : class, IPlayerItem, new()
    where D : IDeckObject, new()
{
    public SavedDiscardPile<D>? PublicDiscardData { get; set; }
    public BasicList<int> PublicDeckList { get; set; } = new();//this is for the deck (cards needing to be drawn)
    public bool AlreadyDrew { get; set; }
    public int PreviousCard { get; set; }
    public int CurrentCard { get; set; }
}