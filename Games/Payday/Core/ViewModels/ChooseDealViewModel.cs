namespace Payday.Core.ViewModels;
public class ChooseDealViewModel : BasicSubmitViewModel
{
    private readonly PaydayVMData _model;
    private readonly IDealProcesses _processes;
    public ChooseDealViewModel(CommandContainer commandContainer, PaydayVMData model, IDealProcesses processes, IEventAggregator aggregator) : base(commandContainer, aggregator)
    {
        _model = model;
        _processes = processes;
    }
    public override bool CanSubmit => _model.PopUpChosen != "";
    public override Task SubmitAsync()
    {
        return _processes.ChoseWhetherToPurchaseDealAsync();
    }
}