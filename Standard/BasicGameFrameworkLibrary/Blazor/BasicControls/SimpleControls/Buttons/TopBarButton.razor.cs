namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls.Buttons;
public partial class TopBarButton : IDisposable
{
    [Parameter]
    public EventCallback CustomCallBack { get; set; }
    [Parameter]
    public string Display { get; set; } = "";
    [Parameter]
    public ICustomCommand? CommandObject { get; set; }
    [Parameter]
    public object? CommandParameter { get; set; }
    private CommandContainer? _commandContainer;
    private bool? _lastCanExecute;
    private bool GetCanExecute()
    {
        if (CustomCallBack.HasDelegate || CommandObject is null)
        {
            return true;
        }

        return CommandObject.CanExecute(CommandParameter);
    }
    protected override void OnParametersSet()
    {

        _lastCanExecute = GetCanExecute();
        base.OnParametersSet();
    }
    protected override void OnInitialized()
    {
        if (Resolver is null)
        {
            return;
        }
        _commandContainer = Resolver!.Resolve<CommandContainer>();
        CommandObject?.CanExecuteChanged += Command_CanExecuteChanged;
        if (CommandObject is not null)
        {
            bool rets = CommandObject.CanExecute(CommandParameter);
            if (CommandObject is IGameCommand game)
            {
                _commandContainer.AddCommand(game);
            }
        }
        base.OnInitialized();
    }
    private void Command_CanExecuteChanged()
    {
        bool current = GetCanExecute();

        if (_lastCanExecute == current)
        {
            return;
        }

        _lastCanExecute = current;
        InvokeAsync(StateHasChanged);
    }
    private bool IsDisabled()
    {
        if (CustomCallBack.HasDelegate || CommandObject is null)
        {
            return false;
        }
        bool rets = CommandObject.CanExecute(CommandParameter);
        return rets == false;
    }
    private async Task Submit()
    {
        if (CustomCallBack.HasDelegate)
        {
            await CustomCallBack.InvokeAsync();
            return;
        }
        if (CommandObject == null)
        {
            return;
        }
        if (CommandObject.CanExecute(CommandParameter) == false)
        {
            return;
        }
        await CommandObject.ExecuteAsync(CommandParameter);
    }
    public void Dispose()
    {
        if (Resolver is null)
        {
            return;
        }
        if (CommandObject is IGameCommand other)
        {
            _commandContainer!.RemoveCommand(other);
        }
        CommandObject?.CanExecuteChanged -= Command_CanExecuteChanged;
        GC.SuppressFinalize(this);
    }
}