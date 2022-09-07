using System.Reflection; //not common enough.
namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.Base;
public abstract class GraphicsCommand : KeyComponentBase, IDisposable
{
    private bool _disposedValue;
    /// <summary>
    /// this is the view model being used.  can be main view model, deck, etc.  this is not the card, etc.
    /// </summary>
    [Parameter]
    public object? CommandParameter { get; set; }
    [Parameter]
    public Action? AfterChange { get; set; }
    protected Assembly GetAssembly => Assembly.GetAssembly(GetType())!; //needs reflection namespace for the images.  decided to not attempt to string based on problems i ran across.
    [Parameter]
    public ICustomCommand? CommandObject { get; set; } //you always need this now.
    private CommandContainer? _commandContainer;
    private void RunProcess()
    {
        if (AfterChange is not null)
        {
            AfterChange.Invoke();
            return;
        }
        StateHasChanged();
    }
    protected override void OnInitialized()
    {
        if (CommandObject is not null)
        {
            _commandContainer = Resolver!.Resolve<CommandContainer>();
            CommandObject.UpdateBlazor = RunProcess;
            if (CommandObject is IGameCommand game)
            {
                _commandContainer.AddCommand(game);
            }
        }
        base.OnInitialized();
    }
    public GraphicsCommand()
    {
        CustomCanDo = PrivateTo;
    }
    private bool PrivateTo() => CanProcess;
    public Func<bool> CustomCanDo { get; set; }
    public bool CanProcess
    {
        get
        {
            if (CommandObject == null)
            {
                return false;
            }
            return CommandObject.CanExecute(CommandParameter);
        }
    }
    public void CreateClick(ISvg svg)
    {
        if (CommandObject != null)
        {
            svg.EventData.ActionClicked = Clicked;
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
        catch (Exception ex)
        {
            string partialTrace;
            if (ex.StackTrace is not null)
            {
                string trace = ex.StackTrace;
                if (trace.Length < 1000)
                {
                    partialTrace = trace;
                }
                else
                {
                    partialTrace = ex.StackTrace.Substring(0, 1000);
                }
            }
            else
            {
                partialTrace = "";
            }
            await BlazorUIHelpers.MessageBox!.ShowMessageAsync($"There was an error.  The error was {ex.Message}.  Stack Trace Was {partialTrace}");
        }
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (_commandContainer is null)
                {
                    return;
                }
                if (CommandObject is IGameCommand other)
                {
                    _commandContainer!.RemoveCommand(other);
                }
            }
            _disposedValue = true;
        }
    }
    public virtual void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
