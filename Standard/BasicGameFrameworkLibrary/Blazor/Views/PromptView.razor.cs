namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class PromptView<VM> : IHandle<OpenPromptEventModel>,
    IHandle<ClosePromptEventModel>, IDisposable
    where VM : class, IPromptVM
{
    private readonly IEventAggregator _aggregator;
    private VM? _dataContext;
    public PromptView()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
        Subscribe();
    }
    protected override void OnInitialized()
    {
        _aggregator.Publish(new AskPromptEventModel());
    }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    private partial void Subscribe();
    private partial void Unsubscribe();
    void IDisposable.Dispose()
    {
        Unsubscribe();
        GC.SuppressFinalize(this);
    }
    void IHandle<OpenPromptEventModel>.Handle(OpenPromptEventModel message)
    {
        _dataContext = (VM)message.DataContext;
        InvokeAsync(StateHasChanged);
    }
    void IHandle<ClosePromptEventModel>.Handle(ClosePromptEventModel message)
    {
        _dataContext = null;
        InvokeAsync(StateHasChanged);
    }
}