namespace CrazyEights.Core.ViewModels;
[InstanceGame]
public class ChooseSuitViewModel : ScreenViewModel, IBlankGameVM
{
    private readonly CrazyEightsVMData _model;
    private readonly ISuitProcesses _processes;
    public ChooseSuitViewModel(CommandContainer commandContainer, CrazyEightsVMData model, ISuitProcesses processes, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _model = model;
        _processes = processes;
        _model.SuitChooser.ItemClickedAsync += SuitChooser_ItemClickedAsync;
    }
    private Task SuitChooser_ItemClickedAsync(EnumSuitList piece)
    {
        return _processes.SuitChosenAsync(piece);
    }
    public CommandContainer CommandContainer { get; set; }
    protected override Task TryCloseAsync()
    {
        _model.SuitChooser.ItemClickedAsync -= SuitChooser_ItemClickedAsync;
        return base.TryCloseAsync();
    }
}