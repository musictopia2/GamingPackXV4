namespace HitTheDeck.Core.Data;
[SingletonGame]
public class HitTheDeckSaveInfo : BasicSavedCardClass<HitTheDeckPlayerItem, HitTheDeckCardInformation>, IMappable, ISaveInfo
{
    public bool HasDrawn { get; set; }
    public bool WasFlipped { get; set; }
    private string _nextPlayer = "";
    public string NextPlayer
    {
        get { return _nextPlayer; }
        set
        {
            if (SetProperty(ref _nextPlayer, value))
            {
                if (_model != null)
                {
                    _model.NextPlayer = value;
                }
            }
        }
    }
    private HitTheDeckVMData? _model;
    public void LoadMod(HitTheDeckVMData model)
    {
        _model = model;
        _model.NextPlayer = NextPlayer;
    }
}