namespace BasicGameFrameworkLibrary.Blazor.Views;
public abstract class BasicGameView<V> : KeyComponentBase
    where V : ScreenViewModel, IBlankGameVM
{
    //this is for subscreens.
    [CascadingParameter]
    public V? DataContext { get; set; }
    protected void ShowChange() //decided to make it protected so if somehow it needs to refresh, can be done.
    {
        InvokeAsync(StateHasChanged);
    }
}