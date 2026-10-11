namespace Candyland.Core.ViewModels;
[InstanceGame]
[EventAggregatorFromMemberClass(nameof(Aggregator))]
public partial class CandylandMainViewModel(CommandContainer commandContainer,
        CandylandMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        CandylandVMData data) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator),
    IHandle<CandylandRefreshEvent>, IDisposable
{
    private partial void Subscribe();
    private partial void Unsubscribe();
    protected override void AdditionalInitialize()
    {
        Unsubscribe(); //just in case.
        Subscribe();
        base.AdditionalInitialize();
    }
    public CandylandVMData VMData { get; set; } = data;
    public BasicList<CandylandPlayerItem> GetPlayerList
    {
        get
        {
            BasicList<CandylandPlayerItem> output = mainGame.PlayerList.ToBasicList();
            output.RemoveSpecificItem(mainGame.SingleInfo!);
            output.Add(mainGame.SingleInfo!);
            return output;
        }
    }
    public CandylandPlayerItem CurrentPlayer => mainGame.SingleInfo!;
    public CandylandCardData CurrentCard => mainGame.SaveRoot.CurrentCard!;
    void IHandle<CandylandRefreshEvent>.Handle(CandylandRefreshEvent message)
    {
        OnStateChanged();
    }

    void IDisposable.Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }
}