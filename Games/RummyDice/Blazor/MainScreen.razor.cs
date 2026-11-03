namespace RummyDice.Blazor;
public partial class MainScreen : IHandle<OpenMainEventModel>,
    IHandle<CloseMainEventModel>, IDisposable
{
    private readonly IEventAggregator _aggregator;
    private RummyDiceMainViewModel? _dataContext;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public MainScreen()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
        if (ll1.LayoutMode == EnumGameLayoutMode.Compact)
        {
            gg1.ButtonSize = BasicGameFrameworkLibrary.Blazor.Configuration.EnumSize.Small;
            gg1.LabelSize = BasicGameFrameworkLibrary.Blazor.Configuration.EnumSize.Small;
        }
        Subscribe();
    }
    protected override void OnInitialized()
    {
        _aggregator.Publish(new AskMainEventModel());
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    void IDisposable.Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }
    void IHandle<OpenMainEventModel>.Handle(OpenMainEventModel message)
    {
        _dataContext = (RummyDiceMainViewModel)message.DataContext;
        InvokeAsync(StateHasChanged);
    }

    void IHandle<CloseMainEventModel>.Handle(CloseMainEventModel message)
    {
        _dataContext = null;
        InvokeAsync(StateHasChanged);
    }
}