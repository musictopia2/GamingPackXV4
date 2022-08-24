namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls;
public partial class DiceListConrolBlazor<D> : IDisposable
    where D : IStandardDice, new()
{
    [Parameter]
    public DiceCup<D>? Cup { get; set; }
    [Parameter]
    public Action? OnChange { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = "15vh"; //decide what the defaults should be.  can always adjust so some games can be larger and some games not as large.
    private readonly CommandContainer _command;
    private bool _disposedValue;
    public DiceListConrolBlazor()
    {
        _command = Resolver!.Resolve<CommandContainer>();
    }
    protected override void OnInitialized()
    {
        _command.AddAction(ShowChange, Cup!.CommandActionString);
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
                _command.RemoveAction(Cup!.CommandActionString); //no need for the action since this uses a dictionary.
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