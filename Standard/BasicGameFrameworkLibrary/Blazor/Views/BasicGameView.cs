namespace BasicGameFrameworkLibrary.Blazor.Views;
public abstract class BasicGameView<V> : KeyComponentBase
    where V : ScreenViewModel, IBlankGameVM
{
    //private bool _disposedValue;

    //this is for subscreens.
    //worked really well so far
    [CascadingParameter]
    public V? DataContext { get; set; }
    //protected override void OnInitialized()
    //{
    //    base.OnInitialized();
    //}
    //protected override void OnParametersSet()
    //{
    //    DataContext!.CommandContainer.AddAction(ShowChange);
    //    base.OnParametersSet();
    //}
    protected void ShowChange() //decided to make it protected so if somehow it needs to refresh, can be done.
    {
        InvokeAsync(StateHasChanged);
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