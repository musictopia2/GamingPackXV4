namespace BasicGameFrameworkLibrary.Blazor.Views;
public abstract class ParameterGameView<VM> : KeyComponentBase
{
    [Parameter]
    [EditorRequired]
    public VM? DataContext { get; set; }
    protected void ShowChange() //decided to make it protected so if somehow it needs to refresh, can be done.
    {
        InvokeAsync(StateHasChanged);
    }
}