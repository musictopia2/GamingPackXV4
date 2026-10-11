namespace Mastermind.Core.ViewModels;
[InstanceGame]
public partial class MastermindOpeningViewModel(CommandContainer container,
        IGamePackageResolver resolver,
        LevelClass global,
        IEventAggregator aggregator,
        IMessageBox message) : IPromptVM,
    IHandleAsync<OpenPromptEventModel>, IHandleAsync<ClosePromptEventModel>
{
    public int LevelChosen { get; set; } = 3;
    public ListViewPicker LevelPicker = null!;
    [Command(EnumCommandCategory.Old)]
    public async Task LevelInformationAsync() //can't be static anymore because we need the message.
    {
        await message.ShowMessageAsync("Level 1 has 4 possible colors that are only used once" + Constants.VBCrLf + "Level 2 has 4 possible colors that can be used more than once" + Constants.VBCrLf + "Level 3 has 6 possible colors that can only be used once" + Constants.VBCrLf + "Level 4 has 6 possible colors that can be used more than once" + Constants.VBCrLf + "Level 5 has 8 possible colors that can be used once" + Constants.VBCrLf + "Level 6 has 8 possible colors that can be used more than once");
    }
    partial void CreateCommands();
    private Task LevelPicker_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        LevelChosen = selectedIndex;
        global.LevelChosen = selectedIndex;
        return Task.CompletedTask;
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    void IPromptVM.Initialize()
    {
        Subscribe();
        LevelPicker = new(container, resolver);
        LevelPicker.SelectionMode = ListViewPicker.EnumSelectionMode.SingleItem;
        LevelPicker.IndexMethod = ListViewPicker.EnumIndexMethod.OneBased;
        LevelChosen = global.LevelChosen;
        LevelPicker.LoadTextList(["Level 1", "Level 2", "Level 3", "Level 4", "Level 5", "Level 6"]);
        LevelPicker.ItemSelectedAsync = LevelPicker_ItemSelectedAsync;
        LevelPicker.SelectSpecificItem(LevelChosen);
        LevelPicker.IsEnabled = true;
        CreateCommands();
    }

    async Task IHandleAsync<OpenPromptEventModel>.HandleAsync(OpenPromptEventModel message)
    {
        
    }

    async Task IHandleAsync<ClosePromptEventModel>.HandleAsync(ClosePromptEventModel message)
    {
        Unsubscribe();
    }
}