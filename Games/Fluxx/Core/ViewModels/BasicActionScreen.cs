namespace Fluxx.Core.ViewModels;
public abstract partial class BasicActionScreen : ScreenViewModel, IBlankGameVM, IShowKeeperVM
{
    private readonly KeeperContainer _keeperContainer;
    private readonly FluxxDelegates _delegates;
    public BasicActionScreen(FluxxGameContainer gameContainer,
        ActionContainer actionContainer,
        KeeperContainer keeperContainer,
        FluxxDelegates delegates,
        IFluxxEvent fluxxEvent,
        BasicActionLogic basicActionLogic,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        GameContainer = gameContainer;
        ActionContainer = actionContainer;
        _keeperContainer = keeperContainer;
        _delegates = delegates;
        FluxxEvent = fluxxEvent;
        BasicActionLogic = basicActionLogic;
        CommandContainer = gameContainer.Command;
        ButtonChooseCardVisible = ActionContainer.ButtonChooseCardVisible;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    ICustomCommand IShowKeeperVM.ShowKeepersCommand => ShowKeepersCommand!;
    public CommandContainer CommandContainer { get; set; }
    protected FluxxGameContainer GameContainer { get; }
    protected ActionContainer ActionContainer { get; }
    protected IFluxxEvent FluxxEvent { get; }
    protected BasicActionLogic BasicActionLogic { get; }
    [Command(EnumCommandCategory.Plain)]
    public async Task ShowKeepersAsync()
    {
        if (_delegates.LoadKeeperScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is loading the keeper screen.  Rethink");
        }
        _keeperContainer.ShowKeepers();
        await _delegates.LoadKeeperScreenAsync.Invoke(_keeperContainer);
    }
    public bool ButtonChooseCardVisible { get; set; }
}