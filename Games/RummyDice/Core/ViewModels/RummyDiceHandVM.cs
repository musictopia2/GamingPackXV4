namespace RummyDice.Core.ViewModels;
public partial class RummyDiceHandVM : SimpleControlObservable
{
    public ControlCommand? BoardCommand { get; set; }
    public ControlCommand? DiceCommand { get; set; }

    public BasicList<RummyDiceInfo> HandList = new();
    public int Index { get; set; } //index is needed so it puts to correct one.
    private readonly RummyDiceMainGameClass _mainGame;
    [Command(EnumCommandCategory.Control, Name = nameof(BoardCommand))]
    private async Task PrivateSetChosenAsync()
    {
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendAllAsync("setchosen", Index);
        }
        await _mainGame.SetProcessAsync(Index);
    }
    [Command(EnumCommandCategory.Control, Name = nameof(DiceCommand))]
    private async Task DiceChosenAsync(RummyDiceInfo dice)
    {
        int x = HandList.IndexOf(dice);
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            SendSet thisSet = new();
            thisSet.WhichSet = Index;
            thisSet.Dice = x;
            await _mainGame.Network!.SendAllAsync("diceset", thisSet);
        }
        await SelectUnselectDiceAsync(x);
    }
    public RummyDiceHandVM(CommandContainer command, RummyDiceMainGameClass mainGame) : base(command)
    {
        _mainGame = mainGame;
        CreateCommands();
    }
    partial void CreateCommands();
    public async Task SelectUnselectDiceAsync(int index)
    {
        HandList[index].IsSelected = !HandList[index].IsSelected;
        await _mainGame.ContinueTurnAsync();
    }
    protected override void EnableChange()
    {
        BoardCommand!.ReportCanExecuteChange();
        DiceCommand!.ReportCanExecuteChange(); //i think you have to do manually since its a controlcommand.
    }
    protected override void PrivateEnableAlways() { }

    public void PopulateTiles(BasicList<RummyDiceInfo> thisList)
    {
        HandList.ReplaceRange(thisList);
        HandList.Sort(); //i think
    }
    public void TransferTiles(BasicList<RummyDiceInfo> thisList)
    {
        HandList.AddRange(thisList);
        HandList.ForEach(Items => Items.IsSelected = false); //make sure its all unselected.
        HandList.Sort();
    }
    public BasicList<RummyDiceInfo> GetSelectedDiceAndRemove() //i think returning an interface is acceptable.
    {
        return HandList.RemoveAllAndObtain(xx => xx.IsSelected == true).ToBasicList();
    }
    public void EndTurn()
    {
        HandList.Clear();
    }
}
