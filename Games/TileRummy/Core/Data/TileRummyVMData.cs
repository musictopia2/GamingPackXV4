namespace TileRummy.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class TileRummyVMData : IViewModelData, IEnableAlways
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public PoolCP Pool1;
    public TempSetsObservable<EnumColorType, EnumColorType, TileInfo> TempSets;
    public MainSets MainSets1;
    public TileHand PlayerHand1;
    private readonly TileRummyGameContainer _gameContainer;

    //the real view model will do the code behind for this.
    //means delegates here too.
    //the real view model knows about this but this can't reference view model or overflow errors.
    public Func<int, int, int, Task>? MainSetsClickedAsync { get; set; }
    public Func<int, Task>? TempSetsClickedAsync { get; set; }
    public TileRummyVMData(CommandContainer command, IGamePackageResolver resolver, TileShuffler shuffle, TileRummyGameContainer gameContainer)
    {
        TempSets = new(command, resolver)
        {
            HowManySets = 4
        };
        TempSets.SetClickedAsync = TempSets_SetClickedAsync;
        MainSets1 = new MainSets(command);
        MainSets1.SetClickedAsync = MainSets1_SetClickedAsync;
        PlayerHand1 = new TileHand(command);
        PlayerHand1.ManualSelectUnselect = PlayerHand1_ManualSelectUnselect;
        Pool1 = new PoolCP(command, resolver, shuffle);
        _gameContainer = gameContainer;
    }

    private void PlayerHand1_ManualSelectUnselect(TileInfo payLoad)
    {
        
        var player = _gameContainer.PlayerList!.GetSelf(); //has to be self for this for now.
        //since you can do out of turn, does not matter.
        var card = player.MainHandList.GetSpecificItem(payLoad.Deck);
        card.IsSelected = payLoad.IsSelected; //trying to do manually.
    }

    private Task MainSets1_SetClickedAsync(int setNumber, int section, int deck)
    {
        if (MainSetsClickedAsync == null)
        {
            throw new CustomBasicException("Main sets function not created.  Rethink");
        }
        return MainSetsClickedAsync.Invoke(setNumber, section, deck);
    }
    private Task TempSets_SetClickedAsync(int index)
    {
        if (TempSetsClickedAsync == null)
        {
            throw new CustomBasicException("Temp sets function not created.  Rethink");
        }
        return TempSetsClickedAsync.Invoke(index);
    }
    bool IEnableAlways.CanEnableAlways()
    {
        return true;
    }
}