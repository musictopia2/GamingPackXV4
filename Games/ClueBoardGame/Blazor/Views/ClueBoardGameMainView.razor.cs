namespace ClueBoardGame.Blazor.Views;
public partial class ClueBoardGameMainView
{
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<LabelGridModel> _clues = [];
    private GameBoardGraphicsCP? _graphicsData;
    private PlayerCollection<ClueBoardGamePlayerItem> _players = [];
    protected override void OnInitialized()
    {
        _graphicsData = aa1.Resolver!.Resolve<GameBoardGraphicsCP>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(ClueBoardGameVMData.NormalTurn));
        _clues.Clear();
        _clues.AddLabel("Room", nameof(ClueBoardGameVMData.CurrentRoomName))
            .AddLabel("Character", nameof(ClueBoardGameVMData.CurrentCharacterName))
            .AddLabel("Weapon", nameof(ClueBoardGameVMData.CurrentWeaponName));
        DataContext!.PopulateDetectiveNoteBook();
        _players = _graphicsData.GameContainer!.PlayerList!;
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand RollCommand => DataContext!.RollDiceCommand!;
    private ICustomCommand PredictCommand => DataContext!.MakePredictionCommand!;
    private ICustomCommand AccusationCommand => DataContext!.MakeAccusationCommand!;
    private ICustomCommand StartOverCommand => DataContext!.StartOverCommand!;
    private string GetColor()
    {
        var player = _graphicsData!.GameContainer!.PlayerList!.GetWhoPlayer();
        return player.Color.Color;
    }
}