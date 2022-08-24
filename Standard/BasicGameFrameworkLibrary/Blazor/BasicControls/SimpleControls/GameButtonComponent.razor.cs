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
    [Parameter]
    public Action? AfterChange { get; set; }
    private CommandContainer? _commandContainer;
    protected override void OnInitialized()
    {
        if (Resolver is null)
        {
            return;
        }
        _commandContainer = Resolver!.Resolve<CommandContainer>(); //try this way.
        if (CommandObject is not null)
        {

            CommandObject.UpdateBlazor = RunProcess;
            if (CommandObject is IGameCommand game)
            {
                _commandContainer.AddCommand(game);
            }
        }
        base.OnInitialized();
    }
    private void RunProcess()
    {
        if (AfterChange is not null)
        {
            AfterChange.Invoke();
            return;
        }
        StateHasChanged();
    }
    [Parameter]
    public string FontSize { get; set; } = "3vh";
    [Parameter]
    public string BackgroundColor { get; set; } = cs.Aqua;
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
            return cs.Gray.ToWebColor();
        }
        return cs.Navy.ToWebColor();
    }
    private bool IsDisabled()
    {
        if (CustomCallBack.HasDelegate || CommandObject is null)
        {
            return false; //because a callback has to always allow or if we don't even have commandboject.
        }
        bool rets = CommandObject.CanExecute(CommandParameter);
        return rets == false;
    }
    private async Task Submit()
    {
        if (CustomCallBack.HasDelegate)
        {
            await CustomCallBack.InvokeAsync(); //in this case, always allow no matter what.
            return;
        }
        if (CommandObject == null)
        {
            return; //nothing to submit
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
                //CommandContainer command = aa.Resolver.Resolve<CommandContainer>();
                if (CommandObject is IGameCommand other)
                {
                    _commandContainer!.RemoveCommand(other);
                }
                //else
                //{
                //    _commandContainer!.RemoveAction();
                //}
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