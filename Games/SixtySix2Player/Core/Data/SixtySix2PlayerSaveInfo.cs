namespace SixtySix2Player.Core.Data;
[SingletonGame]
public class SixtySix2PlayerSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem>, IMappable, ISaveInfo
{
    public DeckRegularDict<SixtySix2PlayerCardInformation> CardList { get; set; } = new DeckRegularDict<SixtySix2PlayerCardInformation>();
    public BasicList<int> CardsForMarriage { get; set; } = new();
    public int LastTrickWon { get; set; }
    private SixtySix2PlayerVMData? _model;
    public void LoadMod(SixtySix2PlayerVMData model)
    {
        _model = model;
        _model.BonusPoints = BonusPoints;
    }
    private int _bonusPoints;
    public int BonusPoints
    {
        get { return _bonusPoints; }
        set
        {
            if (SetProperty(ref _bonusPoints, value))
            {
                if (_model != null)
                {
                    _model.BonusPoints = value;
                }
            }
        }
    }
}