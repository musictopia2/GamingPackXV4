namespace Minesweeper.Core.ViewModels;
[InstanceGame]
public class MinesweeperOpeningViewModel : ScreenViewModel, ILevelVM, IMainScreen, IBlankGameVM
{
    private EnumLevel _levelChosen = EnumLevel.Easy;
    public EnumLevel LevelChosen
    {
        get { return _levelChosen; }
        set
        {
            if (SetProperty(ref _levelChosen, value))
            {
                this.PopulateMinesNeeded();
                _global.Level = value;
            }
        }
    }
    public int HowManyMinesNeeded { get; set; }
    public CommandContainer CommandContainer { get; set; }
    public ListViewPicker LevelPicker;
    private readonly LevelClass _global;
    public MinesweeperOpeningViewModel(CommandContainer container, 
        IGamePackageResolver resolver,
        LevelClass global,
        IEventAggregator aggregator) : base(aggregator)
    {
        LevelPicker = new ListViewPicker(container, resolver); //hopefully this simple.
        CommandContainer = container;
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
    private Task LevelPicker_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        LevelChosen = EnumLevel.FromValue(selectedIndex);
        return Task.CompletedTask;
    }
}