namespace MonopolyDicedGame.Core.ViewModels;
[InstanceGame]
public partial class MonopolyDicedGameMainViewModel : BasicMultiplayerMainVM
{
    private readonly MonopolyDicedGameMainGameClass _mainGame; //if we don't need, delete.
    private readonly MonopolyDiceSet _monopolyDice;
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
        _mainGame = mainGame;
        VMData = data;
        _monopolyDice = monopolyDice;
        _toast = toast;
        CreateCommands(commandContainer);
    }
    //anything else needed is here.

    partial void CreateCommands(CommandContainer command);
    public bool CanRoll => _mainGame.SaveRoot.NumberOfCops < 3;

    [Command(EnumCommandCategory.Game)]
    public async Task RollAsync()
    {
        //will be a command now to roll the dice (getting closer to reals).
        if (_monopolyDice.HasSelectedDice())
        {
            _toast.ShowUserErrorToast("Need to either unselect the dice or use them.");
            return;
        }
        await _mainGame.RollDiceAsync();
    }

}