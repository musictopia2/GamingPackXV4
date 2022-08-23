namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.MainViewModels;
public abstract partial class BasicMultiplayerMainVM : ConductorViewModel, ISimpleGame
{
    public RestoreViewModel? RestoreScreen { get; set; }
    private readonly IEndTurn _mainGame;
    private readonly BasicData _basicData;
    private readonly TestOptions _test;
    private readonly IGamePackageResolver _resolver;
    private readonly IGameNetwork? _network;
    public BasicMultiplayerMainVM(CommandContainer commandContainer,
        IEndTurn mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        CommandContainer.IsExecuting = true; //has to be proven false.
        CommandContainer.ManuelFinish = true;
        _mainGame = mainGame;
        _basicData = basicData;
        _test = test;
        _resolver = resolver;
        if (_basicData.MultiPlayer)
        {
            _network = resolver.Resolve<IGameNetwork>();
        }
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer container);
    protected override Task TryCloseAsync()
    {
        return base.TryCloseAsync();
    }
    protected override async Task ActivateAsync()
    {
        if (_test.SaveOption == EnumTestSaveCategory.RestoreOnly)
        {
            if (_basicData.MultiPlayer == false || _basicData.Client == false)
            {
                RestoreScreen = _resolver.Resolve<RestoreViewModel>();
                await LoadScreenAsync(RestoreScreen);
            }
        }
        await base.ActivateAsync(); //now we have to do the base as well.
    }
    public virtual bool CanEnableAlways()
    {
        return true;
    }
    public virtual bool CanEnableBasics()
    {
        return true;
    }
    protected override Task CloseSpecificChildAsync(IScreen childViewModel)
    {
        CommandContainer.RemoveOldItems(childViewModel);
        return base.CloseSpecificChildAsync(childViewModel);
    }
    public CommandContainer CommandContainer { get; set; }
    public virtual bool CanEndTurn()
    {
        return true; //on default can end turn.  but there are exceptions.
    }
    [Command(EnumCommandCategory.Game)]
    public virtual async Task EndTurnAsync() //forgot needs to be virtual.  since that replaces the endturnprocess now.
    {
        if (_basicData.MultiPlayer)
        {
            await _network!.SendEndTurnAsync();
        }
        await _mainGame.EndTurnAsync();
    }
    protected virtual Task PreviewEndTurnMultiplayerAsync()
    {
        return Task.CompletedTask;
    }
}