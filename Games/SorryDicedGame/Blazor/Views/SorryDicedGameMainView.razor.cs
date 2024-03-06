namespace SorryDicedGame.Blazor.Views;
public partial class SorryDicedGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private readonly BasicList<ScoreColumnModel> _scores = [];
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(SorryDicedGameVMData.NormalTurn))
                .AddLabel("Instructions", nameof(SorryDicedGameVMData.Instructions))
                .AddLabel("Status", nameof(SorryDicedGameVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Color", true, nameof(SorryDicedGamePlayerItem.Color));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand RollCommand => DataContext!.RollCommand!;
    private BasicGameCommand ChooseStartCommand => DataContext!.ChoseStartPieceCommand!;
    private BasicGameCommand DiceCommand => DataContext!.SelectDiceCommand!;
    private BasicGameCommand HomeCommand => DataContext!.HomeCommand!;
    private BasicGameCommand WaitingCommand => DataContext!.WaitingCommand!;
    private BasicList<SorryDicedGamePlayerItem> GetPlayerBoards()
    {
        BasicList<SorryDicedGamePlayerItem> players;

        if (DataContext!.MainGame.BasicData.MultiPlayer == false)
        {
            players = DataContext.MainGame.PlayerList.ToBasicList();
        }
        else
        {
            players = DataContext.MainGame.PlayerList.GetAllPlayersStartingWithSelf();
        }
        players.RemoveAllOnly(x => x.InGame == false);
        return players;
    }
}