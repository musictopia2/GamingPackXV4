namespace BasicGameFrameworkLibrary.Core.ViewModels;
[UseLabelGrid]
public partial class BeginningChooseColorViewModel<E, P>(BeginningColorModel<E, P> model,
        IBeginningColorProcesses<E> processes,
        IEventAggregator aggregator) : IBeginningColorViewModel,
    IHandleAsync<OpenBeginningColorEventModel>, IStateChanged
    where E : struct, IFastEnumColorList<E>
    where P : class, IPlayerWithColor<E>, new()
{
    [LabelColumn]
    public partial string Turn { get; set; } = "";


    partial void OnTurnChanged(string value)
    {
        StateChanged?.Invoke();
    }


    [LabelColumn]
    public partial string Instructions { get; set; } = "";

    public event Action? StateChanged;

    private partial void Subscribe();
    private partial void Unsubscribe();
    public PlayerColorPicker<E, P> GetColorPicker => model.ColorChooser;
    
    private Task ColorChooser_ItemClickedAsync(E piece)
    {
        return processes.ChoseColorAsync(piece);
    }

    Task IHandleAsync<OpenBeginningColorEventModel>.HandleAsync(OpenBeginningColorEventModel message)
    {
        return processes.InitAsync();
    }

    void IBeginningColorViewModel.Initialize()
    {
        processes.SetInstructions = x => Instructions = x;
        processes.SetTurn = x => Turn = x;
        model.ColorChooser.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
        model.ColorChooser.ItemClickedAsync = ColorChooser_ItemClickedAsync;
        Unsubscribe();
        Subscribe();
    }

    void IBeginningColorViewModel.Close()
    {
        model.ColorChooser.ItemClickedAsync = null;
        Unsubscribe();
    }
}