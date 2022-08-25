namespace Mastermind.Core.ViewModels;
[InstanceGame]
public partial class MastermindOpeningViewModel : ScreenViewModel, IBlankGameVM
{
    public int LevelChosen { get; set; } = 3;
    public CommandContainer CommandContainer { get; set; }
    public ListViewPicker LevelPicker;
    private readonly LevelClass _global;
    private readonly IMessageBox _message;
    [Command(EnumCommandCategory.Old)]
    public async Task LevelInformationAsync() //can't be static anymore because we need the message.
    {
        await _message.ShowMessageAsync("Level 1 has 4 possible colors that are only used once" + Constants.VBCrLf + "Level 2 has 4 possible colors that can be used more than once" + Constants.VBCrLf + "Level 3 has 6 possible colors that can only be used once" + Constants.VBCrLf + "Level 4 has 6 possible colors that can be used more than once" + Constants.VBCrLf + "Level 5 has 8 possible colors that can be used once" + Constants.VBCrLf + "Level 6 has 8 possible colors that can be used more than once");
    }
    public MastermindOpeningViewModel(CommandContainer container,
        IGamePackageResolver resolver, 
        LevelClass global, 
        IEventAggregator aggregator,
        IMessageBox message
        ) : base(aggregator)
    {
        LevelPicker = new (container, resolver);
        LevelPicker.SelectionMode = ListViewPicker.EnumSelectionMode.SingleItem;
        LevelPicker.IndexMethod = ListViewPicker.EnumIndexMethod.OneBased;
        _global = global;
        _message = message;
        CommandContainer = container;
        LevelChosen = _global.LevelChosen;
        LevelPicker.LoadTextList(new BasicList<string>() { "Level 1", "Level 2", "Level 3", "Level 4", "Level 5", "Level 6" });
        LevelPicker.ItemSelectedAsync += LevelPicker_ItemSelectedAsync;
        LevelPicker.SelectSpecificItem(LevelChosen);
        LevelPicker.IsEnabled = true;
        CreateCommands();
    }
    partial void CreateCommands();
    private Task LevelPicker_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        LevelChosen = selectedIndex;
        _global.LevelChosen = selectedIndex;
        return Task.CompletedTask;
    }
}