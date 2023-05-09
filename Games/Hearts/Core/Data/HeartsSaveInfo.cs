namespace Hearts.Core.Data;
[SingletonGame]
public class HeartsSaveInfo : BasicSavedTrickGamesClass<EnumSuitList, HeartsCardInformation, HeartsPlayerItem>, IMappable, ISaveInfo, ITrickStatusSavedClass
{
    public EnumTrickStatus TrickStatus { get; set; }
    public int WhoLeadsTrick { get; set; }
    private int _roundNumber;
    public int RoundNumber
    {
        get { return _roundNumber; }
        set
        {
            if (SetProperty(ref _roundNumber, value))
            {
                if (_model != null)
                {
                    _model.RoundNumber = value;
                }
            }
        }
    }
    public EnumPassOption PassOption { get; set; }
    private EnumStatus _gameStatus;
    public EnumStatus GameStatus
    {
        get { return _gameStatus; }
        set
        {
            if (SetProperty(ref _gameStatus, value))
            {
                if (_model == null)
                {
                    return;
                }
                ChangeHand();
            }
        }
    }
    private void ChangeHand()
    {
        if (GameStatus == EnumStatus.Passing)
        {
            _model!.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        }
        else
        {
            _model!.PlayerHand1.AutoSelect = EnumHandAutoType.SelectOneOnly;
        }
    }
    private HeartsVMData? _model;
    public void LoadMod(HeartsVMData model)
    {
        _model = model;
        _model.RoundNumber = RoundNumber;
        ChangeHand();
    }
}