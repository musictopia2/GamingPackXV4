namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public class PlayerSingleHand<D> : SimplePlayer, IPlayerSingleHand<D> where D : IDeckObject, new()
{
    public PlayerSingleHand()
    {
        _mainHandList = [];
        _mainHandList.StateChanged += MainHandListChanged;
    }
    [ScoreColumn]
    public virtual int ObjectCount => MainHandList.Count;
    private DeckRegularDict<D> _mainHandList;
    public DeckRegularDict<D> MainHandList
    {
        get => _mainHandList;
        set
        {
            if (ReferenceEquals(_mainHandList, value))
            {
                return;
            }

            _mainHandList.StateChanged -= MainHandListChanged;

            _mainHandList = value;

            _mainHandList.StateChanged += MainHandListChanged;

            OnScoreValueChanged(nameof(ObjectCount));
        }
    }

    protected void MainHandListChanged()
    {
        OnScoreValueChanged(nameof(ObjectCount));
    }

    [JsonIgnore]
    public DeckRegularDict<D> StartUpList { get; set; } = new DeckRegularDict<D>();
}