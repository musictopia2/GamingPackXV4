namespace SorryDicedGame.Core.ViewModels;
[InstanceGame]
public partial class SorryDicedGameMainViewModel : BasicMultiplayerMainVM
{
    public readonly SorryDicedGameMainGameClass MainGame; //if we don't need, delete.
    private readonly IToast _toast;
    public SorryDicedGameVMData VMData { get; set; }
    public SorryDicedGameMainViewModel(CommandContainer commandContainer,
        SorryDicedGameMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        SorryDicedGameVMData data,
        IToast toast
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        MainGame = mainGame;
        VMData = data;
        _toast = toast;
        CreateCommands(commandContainer);
    }
    //anything else needed is here.
    partial void CreateCommands(CommandContainer command);

    [Command(EnumCommandCategory.Game)]
    public async Task RollAsync()
    {
        //will be a command now to roll the dice (getting closer to reals).
        
        await MainGame.RollAsync();
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ChoseStartPiece(EnumColorChoice color)
    {
        _toast.ShowInfoToast(color.ToString());
        await Task.Delay(1);
    }
    public static bool CanSelectDice(SorryDiceModel dice) => dice.IsEnabled; //not sure if i can do static or not (?)
    [Command(EnumCommandCategory.Game)]
    public async Task SelectDiceAsync(SorryDiceModel dice)
    {
        await Task.Delay(1);
        if (dice.IsSelected)
        {
            dice.IsSelected = false;
            return;
        }
        foreach (var item in MainGame.SaveRoot.DiceList)
        {
            item.IsSelected = false;
        }
        dice.IsSelected = true;
        
    }

}