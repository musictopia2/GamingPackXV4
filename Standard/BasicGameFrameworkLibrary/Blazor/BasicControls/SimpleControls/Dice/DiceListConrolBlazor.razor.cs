namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls.Dice;
public partial class DiceListConrolBlazor<D> : IDisposable
    where D : IStandardDice, new()
{
    [Parameter]
    public DiceCup<D>? Cup { get; set; }
    [Parameter]
    public double PercentHeight { get; set; } = 18; //decide what the defaults should be.  can always adjust so some games can be larger and some games not as large.
    private readonly CommandContainer _command;

    private string TargetHeight => PercentHeight.HeightPixels;

    private bool _disposedValue;
    public DiceListConrolBlazor()
    {
        _command = Resolver!.Resolve<CommandContainer>();
    }
    protected override void OnInitialized()
    {
        _command.AddSpecificAction(Cup!.CommandActionString, ShowChange);
    }
    private async void ShowChange()
    {
        await InvokeAsync(StateHasChanged);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _command.RemoveSpecificAction(Cup!.CommandActionString);
            }
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}