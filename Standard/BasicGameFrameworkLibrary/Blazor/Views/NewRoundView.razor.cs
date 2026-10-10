namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class NewRoundView : IHandle<OpenNewRoundEventModel>,
    IHandle<CloseNewRoundEventModel>, IDisposable
{
    private readonly IEventAggregator _aggregator;
    private NewRoundViewModel? _dataContext;
    public NewRoundView()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe();
    }
    protected override void OnInitialized()
    {
        _aggregator.Publish(new AskNewGameEventModel());
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    void IDisposable.Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }
    void IHandle<OpenNewRoundEventModel>.Handle(OpenNewRoundEventModel message)
    {
        _dataContext = message.DataContext;
        InvokeAsync(StateHasChanged);
    }
    void IHandle<CloseNewRoundEventModel>.Handle(CloseNewRoundEventModel message)
    {
        _dataContext = null;
        InvokeAsync(StateHasChanged);
    }
}