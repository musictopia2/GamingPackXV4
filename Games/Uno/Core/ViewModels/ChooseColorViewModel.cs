namespace Uno.Core.ViewModels;
[InstanceGame]
public class ChooseColorViewModel : ScreenViewModel, IBlankGameVM
{
    public readonly UnoVMData Model;
    private readonly IChooseColorProcesses _processes;
    public ChooseColorViewModel(CommandContainer commandContainer, UnoVMData model, IChooseColorProcesses processes, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        Model = model;
        _processes = processes;
        Model.ColorPicker.ItemClickedAsync += ColorPicker_ItemClickedAsync;
    }
    protected override Task TryCloseAsync()
    {
        Model.ColorPicker.ItemClickedAsync -= ColorPicker_ItemClickedAsync;
        return base.TryCloseAsync();
    }
    private async Task ColorPicker_ItemClickedAsync(EnumColorTypes piece)
    {
        await _processes!.ColorChosenAsync(piece);
    }
    public CommandContainer CommandContainer { get; set; }
}