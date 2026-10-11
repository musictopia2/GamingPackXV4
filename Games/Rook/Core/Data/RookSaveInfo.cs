namespace Rook.Core.Data;
[SingletonGame]
[UseStateChanged]
public partial class RookSaveInfo : BasicSavedTrickGamesClass<EnumColorTypes, RookCardInformation, RookPlayerItem>, IMappable, ISaveInfo
{
    public int WonSoFar { get; set; }
    public bool DummyPlay { get; set; }
    public int HighestBidder { get; set; }
    public DeckRegularDict<RookCardInformation> NestList { get; set; } = [];
    public DeckRegularDict<RookCardInformation> DummyList { get; set; } = [];
    public DeckRegularDict<RookCardInformation> CardList { get; set; } = [];

    [StateProperty]
    public partial EnumStatusList GameStatus { get; set; }

    partial void OnGameStatusChanged(EnumStatusList value)
    {
        if (_model is null)
        {
            return;
        }
        _model.GameStatus = value;
    }


    //private EnumStatusList _gameStatus;
    //public EnumStatusList GameStatus
    //{
    //    get { return _gameStatus; }
    //    set
    //    {
    //        if (SetProperty(ref _gameStatus, value))
    //        {
    //            if (_model == null)
    //            {
    //                return;
    //            }
    //            _model.GameStatus = value;
    //        }
    //    }
    //}
    private RookVMData? _model;
    public void LoadMod(RookVMData model)
    {
        _model = model;
        _model.GameStatus = GameStatus;
    }
}