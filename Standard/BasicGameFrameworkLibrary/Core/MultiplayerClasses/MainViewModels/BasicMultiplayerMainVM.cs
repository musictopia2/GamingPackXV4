namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.MainViewModels;
[EventAggregatorFromMemberClass(nameof(Aggregator))]
[UseStateChanged]
public abstract partial class BasicMultiplayerMainVM(CommandContainer commandContainer,
          IEndTurn mainGame,
          BasicData basicData,
          IGamePackageResolver resolver,
          IEventAggregator aggregator) : IMainGameVM, ISimpleGame, 
    IHandle<BeforeGameOverEventModel>,
    IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>
{
    private IGameNetwork? _network;
    protected BasicData BasicData { get; } = basicData;
    protected IGamePackageResolver Resolver = resolver;
    void IMainGameVM.Initialize()
    {
        Subscribe();
        if (BasicData.MultiPlayer)
        {
            _network = Resolver.Resolve<IGameNetwork>();
        }
        CreateCommands(CommandContainer);
        AdditionalInitialize();
    }
    protected IEventAggregator Aggregator { get; set; } = aggregator;
    private partial void Subscribe();
    private partial void Unsubscribe();
    partial void CreateCommands(CommandContainer container);
    protected virtual void AdditionalInitialize() { }
    protected virtual Task CloseAsync() => Task.CompletedTask; //was going to require but found many games where it was not required.
    Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        return OpenAsync();
    }
    protected virtual Task OpenAsync() => Task.CompletedTask;
    async Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        Unsubscribe();
        await CloseAsync();
    }
    public virtual bool CanEnableAlways()
    {
        return true;
    }
    public virtual bool CanEnableBasics()
    {
        return true;
    }
    public CommandContainer CommandContainer { get; set; } = commandContainer;
    public virtual bool CanEndTurn()
    {
        return true;
    }
    [Command(EnumCommandCategory.Game)]
    public virtual async Task EndTurnAsync()
    {
        if (BasicData.MultiPlayer)
        {
            await _network!.SendEndTurnAsync();
        }
        await mainGame.EndTurnAsync();
    }
    protected virtual Task PreviewEndTurnMultiplayerAsync()
    {
        return Task.CompletedTask;
    }

    void IHandle<BeforeGameOverEventModel>.Handle(BeforeGameOverEventModel message)
    {
        OnStateChanged();
    }
}