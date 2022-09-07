namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.SavedGameClasses;
public class BasicSavedTrickGamesClass<S, T, P> : BasicSavedCardClass<P, T>
    where S : IFastEnumSimple
    where T : class, ITrickCard<S>, new()
    where P : class, IPlayerTrick<S, T>, new()
{
    public DeckRegularDict<T> TrickList { get; set; } = new();
    private S? _trumpSuit;
    public S? TrumpSuit
    {
        get { return _trumpSuit!; }
        set
        {
            if (SetProperty(ref _trumpSuit, value))
            {
                if (TrickMod != null)
                {
                    TrickMod.TrumpSuit = value!;
                }
            }
        }
    }
    [JsonIgnore]
    public ITrickCardGamesData<T, S>? TrickMod { get; set; }
    public void LoadTrickVM(ITrickCardGamesData<T, S> TrickMod)
    {
        this.TrickMod = TrickMod;
        TrickMod.TrumpSuit = TrumpSuit!;
    }
}