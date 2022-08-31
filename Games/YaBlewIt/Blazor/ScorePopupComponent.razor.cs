namespace YaBlewIt.Blazor;
public partial class ScorePopupComponent
{
    [Parameter]
    public EventCallback Close { get; set; }

    private void PrivateClose()
    {
        if (Close.HasDelegate == false)
        {
            return;
        }
        Close.InvokeAsync();
    }
}
