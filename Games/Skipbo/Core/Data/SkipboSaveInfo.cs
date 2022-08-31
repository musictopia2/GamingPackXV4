namespace Skipbo.Core.Data;
[SingletonGame]
public class SkipboSaveInfo : BasicSavedCardClass<SkipboPlayerItem, SkipboCardInformation>, IMappable, ISaveInfo
{
    public BasicList<BasicPileInfo<SkipboCardInformation>> PublicPileList { get; set; } = new();
    private int _cardsToShuffle;
    public int CardsToShuffle
    {
        get { return _cardsToShuffle; }
        set
        {
            if (SetProperty(ref _cardsToShuffle, value))
            {
                if (_model == null)
                {
                    return;
                }
                _model.CardsToShuffle = value;
            }
        }
    }
    public void LoadMod(SkipboVMData model)
    {
        _model = model;
        _model.CardsToShuffle = CardsToShuffle;
    }
    private SkipboVMData? _model;
}