namespace Minesweeper.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class MinesweeperOpeningViewModel(CommandContainer container,
    IGamePackageResolver resolver,
    LevelClass global,
    IEventAggregator aggregator
    ) : IPromptVM, ILevelVM,
    IHandleAsync<OpenPromptEventModel>, IHandleAsync<ClosePromptEventModel>
{
    private partial void Subscribe();
    private partial void Unsubscribe(); 
    [StateProperty]
    public partial EnumLevel LevelChosen { get; set; }
    partial void OnLevelChosenChanged(EnumLevel value)
    {
        _global.Level = value;
        this.PopulateMinesNeeded();
    }
    public int HowManyMinesNeeded { get; set; }
    public CommandContainer CommandContainer => container;
    public ListViewPicker LevelPicker = null!;
    private LevelClass _global = null!;
    private Task LevelPicker_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        LevelChosen = EnumLevel.FromValue(selectedIndex);
        return Task.CompletedTask;
    }
    void IPromptVM.Initialize()
    {
        Subscribe();
        LevelPicker = new ListViewPicker(container, resolver); //hopefully this simple.
        LevelPicker.SelectionMode = ListViewPicker.EnumSelectionMode.SingleItem;
        LevelPicker.IndexMethod = ListViewPicker.EnumIndexMethod.OneBased;
        _global = global;
        LevelChosen = _global.Level;
        this.PopulateMinesNeeded();
        LevelPicker.ItemSelectedAsync = LevelPicker_ItemSelectedAsync;
        var list = EnumLevel.CompleteList.Select(x => x.Words).ToBasicList();
        LevelPicker.LoadTextList(list);
        LevelPicker.SelectSpecificItem(LevelChosen.Value);
        LevelPicker.IsEnabled = true;
    }
    async Task IHandleAsync<OpenPromptEventModel>.HandleAsync(OpenPromptEventModel message)
    {
        //not sure if i need anything here now.
    }

    async Task IHandleAsync<ClosePromptEventModel>.HandleAsync(ClosePromptEventModel message)
    {
        Unsubscribe();
    }
}