namespace MonopolyDicedGame.Core.ViewModels;
[InstanceGame]
public partial class MonopolyDicedGameMainViewModel : BasicMultiplayerMainVM
{
    private readonly IToast _toast;
    public MonopolyDicedGameVMData VMData { get; set; }
    public MonopolyDicedGameMainViewModel(CommandContainer commandContainer,
        MonopolyDicedGameMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        MonopolyDicedGameVMData data,
        MonopolyDiceSet monopolyDice,
        IToast toast
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        MainGame = mainGame;
        VMData = data;
        MonopolyDice = monopolyDice;
        _toast = toast;
        CreateCommands(commandContainer);
    }
    //anything else needed is here.
    public MonopolyDicedGameMainGameClass MainGame;
    public MonopolyDiceSet MonopolyDice;
    partial void CreateCommands(CommandContainer command);
    public override bool CanEndTurn() => MainGame.SaveRoot.RollNumber > 1;
    public bool CanRoll => MainGame.SaveRoot.NumberOfCops < 3;
    [Command(EnumCommandCategory.Game)]
    public async Task RollAsync()
    {
        //will be a command now to roll the dice (getting closer to reals).
        if (MonopolyDice.HasSelectedDice())
        {
            _toast.ShowUserErrorToast("Need to either unselect the dice or use them.");
            return;
        }
        await MainGame.RollDiceAsync();
    }

}