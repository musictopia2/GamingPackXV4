namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.Base;
public abstract class GraphicsCommand : KeyComponentBase, IDisposable
{
    private bool _disposedValue;
    private ICustomCommand? _subscribedCommand;

    [Parameter]
    public object? CommandParameter { get; set; }

    [Parameter]
    public bool Fixed { get; set; }

    [Parameter]
    public ICustomCommand? CommandObject { get; set; }

    public GraphicsCommand()
    {
        CustomCanDo = PrivateTo;
    }

    public Func<bool> CustomCanDo { get; set; }

    public bool CanProcess
    {
        get
        {
            if (CommandObject is null)
            {
                return false;
            }

            return CommandObject.CanExecute(CommandParameter);
        }
    }
    protected override bool ShouldRender()
    {
        return Fixed == false;
    }

    protected override void OnParametersSet()
    {
        if (ReferenceEquals(_subscribedCommand, CommandObject) == false)
        {
            UnsubscribeCommand();
            _subscribedCommand = CommandObject;
            _subscribedCommand?.CanExecuteChanged += Command_CanExecuteChanged;
        }

        base.OnParametersSet();
    }
    private void Command_CanExecuteChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private bool PrivateTo()
    {
        return CanProcess;
    }

    public void CreateClick(ISvg svg)
    {
        if (CommandObject is not null)
        {
            svg.EventData.ActionClicked = Clicked;
        }
    }

    public void CreateClick(G group)
    {
        if (CommandObject is not null)
        {
            group.EventData.ActionClicked = Clicked;
        }
    }

    private async Task Clicked(object args1, object args2)
    {
        await Submit();
    }

    protected virtual async Task Submit()
    {
        try
        {
            if (CommandObject is null)
            {
                return;
            }

            if (CommandObject.CanExecute(CommandParameter) == false)
            {
                return;
            }

            await CommandObject.ExecuteAsync(CommandParameter);

            // The command may have changed the model represented
            // by this graphics component.
            //await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {
            string partialTrace;

            if (ex.StackTrace is not null)
            {
                partialTrace = ex.StackTrace.Length < 1000
                    ? ex.StackTrace
                    : ex.StackTrace.Substring(0, 1000);
            }
            else
            {
                partialTrace = "";
            }

            await BlazorUIHelpers.MessageBox!.ShowMessageAsync(
                $"There was an error.  The error was {ex.Message}.  " +
                $"Stack Trace Was {partialTrace}");
        }
    }
    private void UnsubscribeCommand()
    {
        _subscribedCommand?.CanExecuteChanged -= Command_CanExecuteChanged;
        _subscribedCommand = null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            UnsubscribeCommand();
        }

        _disposedValue = true;
    }

    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}