namespace SnakesAndLadders.Core.ViewModels;
[InstanceGame]
public partial class SnakesAndLaddersMainViewModel(CommandContainer commandContainer,
        SnakesAndLaddersMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        SnakesAndLaddersVMData data,
        IToast toast) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public SnakesAndLaddersVMData VMData { get; set; } = data;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public BasicList<SnakesAndLaddersPlayerItem> GetPlayerList
    {
        get
        {
            BasicList<SnakesAndLaddersPlayerItem> output = mainGame.PlayerList.ToBasicList();
            output.RemoveSpecificItem(mainGame.SingleInfo!);
            output.Add(mainGame.SingleInfo!);
            return output;
        }
    }
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public bool CanRollDice => !mainGame.SaveRoot.HasRolled;
    [Command(EnumCommandCategory.Game)]
    public async Task RollDiceAsync()
    {
        await mainGame.Roll.RollDiceAsync();
    }
    public bool CanMakeMove(int space)
    {
        if (space == 0)
        {
            return false;
        }
        return mainGame.SaveRoot.HasRolled;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakeMoveAsync(int space)
    {
        if (mainGame.GameBoard1.IsValidMove(space) == false)
        {
            toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        await mainGame.MakeMoveAsync(space);
    }
}