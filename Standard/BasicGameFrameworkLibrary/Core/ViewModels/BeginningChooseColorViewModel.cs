namespace BasicGameFrameworkLibrary.Core.ViewModels;

[UseLabelGrid]
public partial class BeginningChooseColorViewModel<E, P> : ScreenViewModel, IBlankGameVM, IBeginningColorViewModel, IDisposable
    where E : struct, IFastEnumColorList<E>
    where P : class, IPlayerBoardGame<E>, new()
{
    private readonly BeginningColorModel<E, P> _model;
    private readonly IBeginningColorProcesses<E> _processes;
    private bool _disposedValue;
    [LabelColumn]
    public string Turn { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
    public BeginningChooseColorViewModel(CommandContainer commandContainer,
        BeginningColorModel<E, P> model,
        IBeginningColorProcesses<E> processes,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _model = model;
        _processes = processes;
        _processes.SetInstructions = (x => Instructions = x);
        _processes.SetTurn = (x => Turn = x); //has to set delegates before init obviously.
        _model.ColorChooser.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
        _model.ColorChooser.ItemClickedAsync += ColorChooser_ItemClickedAsync;
    }
    public BoardGamesColorPicker<E, P> GetColorPicker => _model.ColorChooser;
    protected override Task ActivateAsync()
    {
        return _processes.InitAsync();
    }
    private Task ColorChooser_ItemClickedAsync(E piece)
    {
        return _processes.ChoseColorAsync(piece);
    }
    public CommandContainer CommandContainer { get; set; }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _model.ColorChooser.ItemClickedAsync -= ColorChooser_ItemClickedAsync;
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