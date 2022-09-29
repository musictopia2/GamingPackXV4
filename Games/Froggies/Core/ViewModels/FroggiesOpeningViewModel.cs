namespace Froggies.Core.ViewModels;
[SingletonGame]
public class FroggiesOpeningViewModel : ScreenViewModel, IBlankGameVM
{
    public int NumberOfFrogs { get; set; }
    public CommandContainer CommandContainer { get; set; }
    public NumberPicker LevelPicker;
    private readonly LevelClass _global;
    public FroggiesOpeningViewModel(CommandContainer container,
        IGamePackageResolver resolver,
        LevelClass global,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        _global = global;
        CommandContainer = container;
        NumberOfFrogs = _global.NumberOfFrogs;
        LevelPicker = new NumberPicker(container, resolver);
        LevelPicker.LoadNormalNumberRangeValues(3, 60); //maybe 60 at a max is good enough.
        LevelPicker.SelectNumberValue(NumberOfFrogs);
        LevelPicker.ChangedNumberValueAsync = LevelPicker_ChangedNumberValueAsync;
    }
    private Task LevelPicker_ChangedNumberValueAsync(int chosen)
    {
        NumberOfFrogs = chosen;
        _global.NumberOfFrogs = chosen;
        return Task.CompletedTask;
    }
}