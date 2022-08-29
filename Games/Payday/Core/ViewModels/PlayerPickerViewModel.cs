namespace Payday.Core.ViewModels;
public class PlayerPickerViewModel : BasicSubmitViewModel
{
    private readonly PaydayVMData _model;
    private readonly IChoosePlayerProcesses _processes;
    public PlayerPickerViewModel(CommandContainer commandContainer, PaydayVMData model, IChoosePlayerProcesses processes, IEventAggregator aggregator) : base(commandContainer, aggregator)
    {
        _model = model;
        _processes = processes;
    }
    public override bool CanSubmit => _model.PopUpChosen != "";
    public override Task SubmitAsync()
    {
        return _processes.ProcessChosenPlayerAsync();
    }
}