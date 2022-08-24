namespace BasicGameFrameworkLibrary.Blazor.Views;
public partial class BeginningChooseColorView<E, P>
    where E : struct, IFastEnumColorList<E>
    where P : class, IPlayerBoardGame<E>, new()
{
    [Parameter]
    public string TargetSize { get; set; } = "25vh"; //can be adjustable as needed.
    [Parameter]
    public RenderFragment<BasicPickerData<E>>? ChildContent { get; set; } //hopefully this simple
    [CascadingParameter]
    public BeginningChooseColorViewModel<E, P>? DataContext { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    //private bool _disposedValue;
    protected override void OnInitialized()
    {
        _labels.Clear();
        //DataContext!.CommandContainer.AddAction(ShowChange);
        _labels.AddLabel("Turn", nameof(IBeginningColorViewModel.Turn))
            .AddLabel("Instructions", nameof(IBeginningColorViewModel.Instructions));
        base.OnInitialized();
    }
    private void ShowChange()
    {
        InvokeAsync(() => StateHasChanged());
    }
    //protected virtual void Dispose(bool disposing)
    //{
    //    if (!_disposedValue)
    //    {
    //        if (disposing)
    //        {
    //            DataContext!.CommandContainer.RemoveAction(ShowChange);
    //        }
    //        _disposedValue = true;
    //    }
    //}
    //public void Dispose()
    //{
    //    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //    Dispose(disposing: true);
    //    GC.SuppressFinalize(this);
    //}
}