namespace CrazyEights.Core.ViewModels;
[InstanceGame]
public class ChooseSuitViewModel
{
    private readonly CrazyEightsVMData _model;
    private readonly ISuitProcesses _processes;
    public ChooseSuitViewModel(CommandContainer commandContainer, CrazyEightsVMData model, ISuitProcesses processes)
    {
        CommandContainer = commandContainer;
        _model = model;
        _processes = processes;
        _model.SuitChooser.ItemClickedAsync = SuitChooser_ItemClickedAsync;
    }
    private Task SuitChooser_ItemClickedAsync(EnumSuitList piece)
    {
        return _processes.SuitChosenAsync(piece);
    }
    public void Close()
    {
        _model.SuitChooser.ItemClickedAsync = null;
    }
    public CommandContainer CommandContainer { get; set; }
}