namespace RummyDice.Blazor;
public partial class RummyListBlazor : IDisposable
{
    private bool _disposedValue;
    [Parameter]
    public RummyBoardCP? GameBoard { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = ""; //this is the height of the dice being used.
    [CascadingParameter]
    public RummyDiceMainViewModel? DataContext { get; set; }
    private BasicList<RummyDiceInfo> DiceList { get; set; } = new();
    protected override void OnInitialized()
    {
        DataContext!.CommandContainer.AddAction(ShowChange, "rummydice");
        base.OnInitialized();
    }
    private void ShowChange()
    {
        InvokeAsync(StateHasChanged);
    }
    protected override void OnParametersSet()
    {
        DiceList = DataContext!.MainGame.SaveRoot.DiceList;

        base.OnParametersSet();
    }
    private async Task OnClickAsync(RummyDiceInfo dice)
    {
        await GameBoard!.SelectDiceAsync(dice);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                DataContext!.CommandContainer.RemoveAction("rummydice");
            }
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}