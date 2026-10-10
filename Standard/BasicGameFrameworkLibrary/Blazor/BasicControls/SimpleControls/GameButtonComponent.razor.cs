namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls;
public partial class GameButtonComponent : IDisposable
{
    [Parameter]
    public string Width { get; set; } = "";
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
    [Parameter]
    public string FontSize { get; set; } = "3vh";
    [Parameter]
    public string BackgroundColor { get; set; } = cs1.Aqua;
    [Parameter]
    public bool StartOnNewLine { get; set; } = false;
    [Parameter]
    public EventCallback CustomCallBack { get; set; }
    private string ExtraInfo()
    {
        if (Width == "")
        {
            return "";
        }
        return $"width: {Width};";
    }
    private string GetTextColor()
    {
        if (IsDisabled())
        {
            return cs1.Gray.ToWebColor;
        }
        return cs1.Navy.ToWebColor;
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
    private bool _disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
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
                //if (CommandObject is OldCommand old)
                //{
                //    old.CanExecuteChanged -= Command_CanExecuteChanged;
                //}
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