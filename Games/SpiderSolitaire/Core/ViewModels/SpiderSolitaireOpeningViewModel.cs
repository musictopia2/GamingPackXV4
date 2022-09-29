namespace SpiderSolitaire.Core.ViewModels;
[InstanceGame]
public class SpiderSolitaireOpeningViewModel : ScreenViewModel, IBlankGameVM
{
    public int LevelChosen { get; set; } = 3;
    public CommandContainer CommandContainer { get; set; }
    public ListViewPicker LevelPicker;
    private readonly LevelClass _global;
    public SpiderSolitaireOpeningViewModel(CommandContainer container, IGamePackageResolver resolver, LevelClass global, IEventAggregator aggregator) : base(aggregator)
    {
        LevelPicker = new (container, resolver); //hopefully this simple.
        LevelPicker.SelectionMode = ListViewPicker.EnumSelectionMode.SingleItem;
        LevelPicker.IndexMethod = ListViewPicker.EnumIndexMethod.OneBased;
        _global = global;
        CommandContainer = container;
        LevelChosen = _global.LevelChosen;
        LevelPicker.LoadTextList(new() { "1 Suit", "2 Suits", "4 Suits" });
        LevelPicker.ItemSelectedAsync = LevelPicker_ItemSelectedAsync;
        LevelPicker.SelectSpecificItem(LevelChosen);
        LevelPicker.IsEnabled = true;
    }
    private Task LevelPicker_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        LevelChosen = selectedIndex;
        _global.LevelChosen = selectedIndex;
        return Task.CompletedTask;
    }
}