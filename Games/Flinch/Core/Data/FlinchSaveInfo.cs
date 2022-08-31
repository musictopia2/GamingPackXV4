namespace Flinch.Core.Data;
[SingletonGame]
public class FlinchSaveInfo : BasicSavedCardClass<FlinchPlayerItem, FlinchCardInformation>, IMappable, ISaveInfo
{
    public BasicList<BasicPileInfo<FlinchCardInformation>> PublicPileList { get; set; } = new();
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
    public int PlayerFound { get; set; }
    public EnumStatusList GameStatus { get; set; }
    public void LoadMod(FlinchVMData model)
    {
        _model = model;
        _model.CardsToShuffle = CardsToShuffle;
    }
    private FlinchVMData? _model;
}