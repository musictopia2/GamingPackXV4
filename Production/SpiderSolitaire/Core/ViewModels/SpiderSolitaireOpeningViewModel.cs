namespace SpiderSolitaire.Core.ViewModels;
[InstanceGame]
public partial class SpiderSolitaireOpeningViewModel(CommandContainer container,
    IGamePackageResolver resolver,
    LevelClass global,
    IEventAggregator aggregator
    ) : IPromptVM,
    IHandleAsync<OpenPromptEventModel>, IHandleAsync<ClosePromptEventModel>
{

    public int LevelChosen { get; set; } = 3;
    public ListViewPicker LevelPicker = null!;
    private partial void Subscribe();
    private partial void Unsubscribe();
    private Task LevelPicker_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        LevelChosen = selectedIndex;
        global.LevelChosen = selectedIndex;
        return Task.CompletedTask;
    }

    void IPromptVM.Initialize()
    {
        Subscribe();
        LevelPicker = new(container, resolver); //hopefully this simple.
        LevelPicker.SelectionMode = ListViewPicker.EnumSelectionMode.SingleItem;
        LevelPicker.IndexMethod = ListViewPicker.EnumIndexMethod.OneBased;
        LevelChosen = global.LevelChosen;
        LevelPicker.LoadTextList(["1 Suit", "2 Suits", "4 Suits"]);
        LevelPicker.ItemSelectedAsync = LevelPicker_ItemSelectedAsync;
        LevelPicker.SelectSpecificItem(LevelChosen);
        LevelPicker.IsEnabled = true;
    }
    async Task IHandleAsync<OpenPromptEventModel>.HandleAsync(OpenPromptEventModel message)
    {
        
    }
    async Task IHandleAsync<ClosePromptEventModel>.HandleAsync(ClosePromptEventModel message)
    {
        Unsubscribe();
    }
}