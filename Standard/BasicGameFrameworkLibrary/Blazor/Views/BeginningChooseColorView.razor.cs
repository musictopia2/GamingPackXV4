namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class BeginningChooseColorView<E, P>
    where E : struct, IFastEnumColorList<E>
    where P : class, IPlayerBoardGame<E>, new()
{
    [Parameter]
    public string TargetSize { get; set; } = "25vh"; //can be adjustable as needed.
    [Parameter]
    public RenderFragment<BasicPickerData<E>>? ChildContent { get; set; }
    [CascadingParameter]
    public BeginningChooseColorViewModel<E, P>? DataContext { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(IBeginningColorViewModel.Turn))
            .AddLabel("Instructions", nameof(IBeginningColorViewModel.Instructions));
        base.OnInitialized();
    }
}