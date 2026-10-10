namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class BeginningChooseColorView<E, P> : IHandle<OpenBeginningColorEventModel>,
    IHandle<CloseBeginningColorEventModel>, IDisposable
    where E : struct, IFastEnumColorList<E>
    where P : class, IPlayerWithColor<E>, new()
{
    [Parameter]
    public string TargetSize { get; set; } = "25vh"; //can be adjustable as needed.
    [Parameter]
    public RenderFragment<BasicPickerData<E>>? ChildContent { get; set; }
    //[CascadingParameter]
    //public BeginningChooseColorViewModel<E, P>? DataContext { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly IEventAggregator _aggregator;

    private partial void Subscribe();
    private partial void Unsubscribe();
    public BeginningChooseColorView()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
        Unsubscribe();
        Subscribe();
    }

    private BeginningChooseColorViewModel<E, P>? _dataContext;

    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(IBeginningColorViewModel.Turn))
            .AddLabel("Instructions", nameof(IBeginningColorViewModel.Instructions));
        _aggregator.Publish(new AskBeginningColorEventModel());
        base.OnInitialized();
    }

    void IHandle<OpenBeginningColorEventModel>.Handle(OpenBeginningColorEventModel message)
    {
        _dataContext = (BeginningChooseColorViewModel<E,P>) message.DataContext;

        InvokeAsync(StateHasChanged);
    }

    void IHandle<CloseBeginningColorEventModel>.Handle(CloseBeginningColorEventModel message)
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