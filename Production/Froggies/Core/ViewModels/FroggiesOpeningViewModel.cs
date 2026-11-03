namespace Froggies.Core.ViewModels;
[InstanceGame]
public partial class FroggiesOpeningViewModel(CommandContainer container,
    IGamePackageResolver resolver,
    LevelClass global,
    IEventAggregator aggregator
    ) : IPromptVM,
    IHandleAsync<OpenPromptEventModel>, IHandleAsync<ClosePromptEventModel>
{
    private partial void Subscribe();
    private partial void Unsubscribe();
    public int NumberOfFrogs { get; set; }
    public NumberPicker LevelPicker = null!;
    private Task LevelPicker_ChangedNumberValueAsync(int chosen)
    {
        NumberOfFrogs = chosen;
        global.NumberOfFrogs = chosen;
        return Task.CompletedTask;
    }
    async Task IHandleAsync<OpenPromptEventModel>.HandleAsync(OpenPromptEventModel message)
    {
        
    }

    async Task IHandleAsync<ClosePromptEventModel>.HandleAsync(ClosePromptEventModel message)
    {
        Unsubscribe();
    }

    void IPromptVM.Initialize()
    {
        Subscribe();
        NumberOfFrogs = global.NumberOfFrogs;
        LevelPicker = new NumberPicker(container, resolver);
        LevelPicker.LoadNormalNumberRangeValues(3, 60); //maybe 60 at a max is good enough.
        LevelPicker.SelectNumberValue(NumberOfFrogs);
        LevelPicker.ChangedNumberValueAsync = LevelPicker_ChangedNumberValueAsync;
        // V4 did not require this; required with the new lifecycle.
        LevelPicker.IsEnabled = true;
    }
}