namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls.Buttons;
public partial class GameActionButton : IDisposable
{
    [Parameter]
    public bool? UseSuggestedWidth { get; set; }
    [Parameter]
    public EnumSize? SizeCategory { get; set; }

    [Parameter]
    public EnumWidth WidthCategory { get; set; }

    [Parameter]
    public string Display { get; set; } = "";
    [Parameter]
    public ICustomCommand? CommandObject { get; set; }
    [Parameter]
    public object? CommandParameter { get; set; }


    [Parameter]
    public string BackgroundColor { get; set; } = cs1.Aqua; //needed because of games like minesweeper where it may need to change sometimes (you have to choose color where you can still see the text).

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
    public bool StartOnNewLine { get; set; } = false;
    [Parameter]
    public EventCallback CustomCallBack { get; set; }
    //here is where i do guessing.
    private string GetFontSize()
    {
        EnumSize size = gg1.GetButtonSize(SizeCategory);
        if (ll1.LayoutMode == EnumGameLayoutMode.Desktop)
        {
            if (size == EnumSize.Standard)
            {
                return "2.5rem";
            }
            if (size == EnumSize.Large)
            {
                return "5.0rem";
            }
            return "1.5rem";
        }
        if (ll1.LayoutMode == EnumGameLayoutMode.Tablet)
        {
            if (size == EnumSize.Standard)
            {
                return "1.8rem";
            }
            if (size == EnumSize.Large)
            {
                return "4.0rem";
            }
            return "1.0rem";
        }
        if (size == EnumSize.Large)
        {
            return "2.5rem";
        }
        if (size == EnumSize.Small)
        {
            return "1.1rem";
        }
        return "1.3rem";
    }
    //for now, no extra widths.   if i ever need to tweak a little further, then rethink.

    private string GetWidth()
    {
        double width;
        EnumWidth category = gg1.GetButtonWidth(WidthCategory);
        if (ll1.LayoutMode == EnumGameLayoutMode.Desktop)
        {
            width = category switch
            {
                EnumWidth.Narrow => 96,
                EnumWidth.Wide => 400,
                _ => 250
            };
        }
        else if (ll1.LayoutMode == EnumGameLayoutMode.Tablet)
        {
            width = category switch
            {
                EnumWidth.Narrow => 80,
                EnumWidth.Wide => 300,
                _ => 200
            };
        }
        else
        {
            width = category switch
            {
                EnumWidth.Narrow => 64,
                EnumWidth.Wide => 150,
                _ => 96
            };
        }
        EnumSize size = gg1.GetButtonSize(SizeCategory);
        if (size == EnumSize.Large)
        {
            width *= 1.5;
        }
        if (size == EnumSize.Small)
        {
            width *= .80;
        }

        return $"{(int)Math.Ceiling(width)}px";
    }
    private string ExtraInfo()
    {
        string widths = "";

        bool useSuggestion = gg1.GetSuggestedButtonWidth(UseSuggestedWidth);

        if (useSuggestion)
        {
            widths = $"width: {GetWidth()};";
        }


        string fonts = $"font-size: {GetFontSize()};";
        if (useSuggestion)
        {
            return $"{widths} {fonts}";
        }
        return fonts;
        
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