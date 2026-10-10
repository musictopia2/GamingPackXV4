namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class RestoreView : IHandle<OpenRestoreEventModel>,
    IHandle<CloseRestoreEventModel>, IDisposable
{
    private partial void Subscribe();
    private partial void Unsubscribe();
    private readonly IEventAggregator _aggregator;
    private RestoreViewModel? _dataContext;
    public RestoreView()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe();
    }
    protected override void OnInitialized()
    {
        _aggregator.Publish(new AskRestoreEventModel());
    }
    void IHandle<OpenRestoreEventModel>.Handle(OpenRestoreEventModel message)
    {
        _dataContext = message.DataContext;
        InvokeAsync(StateHasChanged);
    }

    void IHandle<CloseRestoreEventModel>.Handle(CloseRestoreEventModel message)
    {
        _dataContext = null;
        InvokeAsync(StateHasChanged);
    }
    void IDisposable.Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }
}