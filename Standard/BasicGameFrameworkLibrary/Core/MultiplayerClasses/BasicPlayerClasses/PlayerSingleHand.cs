namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public class PlayerSingleHand<D> : SimplePlayer, IPlayerSingleHand<D> where D : IDeckObject, new()
{
    [ScoreColumn]
    public virtual int ObjectCount => MainHandList.Count;
    public DeckRegularDict<D> MainHandList { get; set; } = new DeckRegularDict<D>();
    [JsonIgnore]
    public DeckRegularDict<D> StartUpList { get; set; } = new DeckRegularDict<D>();
}