namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class NewGameView : IHandle<OpenNewGameEventModel>,
    IHandle<CloseNewGameEventModel>, IDisposable
{
    private readonly IEventAggregator _aggregator;
    private NewGameViewModel? _dataContext;
    public NewGameView()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe();
    }
    protected override void OnInitialized()
    {
        _aggregator.Publish(new AskNewGameEventModel());
        InProgressHelpers.StateChanged += StateChanged;
        base.OnInitialized();
    }
    private void StateChanged()
    {
        InvokeAsync(StateHasChanged);
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    void IDisposable.Dispose()
    {
        Unsubscribe();
        InProgressHelpers.StateChanged -= StateChanged;
        GC.SuppressFinalize(this);
    }
    void IHandle<OpenNewGameEventModel>.Handle(OpenNewGameEventModel message)
    {
        _dataContext = message.DataContext;
        InvokeAsync(StateHasChanged);
    }
    void IHandle<CloseNewGameEventModel>.Handle(CloseNewGameEventModel message)
    {
        _dataContext = null;
        InvokeAsync(StateHasChanged);
    }
}