namespace SorryDicedGame.Blazor.Views;
public partial class SorryDicedGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(SorryDicedGameVMData.NormalTurn))
                .AddLabel("Instructions", nameof(SorryDicedGameVMData.Instructions))
                .AddLabel("Status", nameof(SorryDicedGameVMData.Status));
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
        return players; //for now will show idle (eventually will remove them.  is needed to make sure there is room for them all.
        //var list = DataContext.MainGame.PlayerList.GetAllPlayersStartingWithSelf();
        //list.RemoveAllOnly(x => x.InGame == false); //if a player is out of the game, then not needed for this.
        //return list;
    }
}