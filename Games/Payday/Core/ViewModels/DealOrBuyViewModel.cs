namespace Payday.Core.ViewModels;
public class DealOrBuyViewModel : BasicSubmitViewModel
{
    private readonly PaydayVMData _model;
    private readonly IDealBuyChoiceProcesses _processes;
    public DealOrBuyViewModel(CommandContainer commandContainer, PaydayVMData model, IDealBuyChoiceProcesses processes, IEventAggregator aggregator) : base(commandContainer, aggregator)
    {
        _model = model;
        _processes = processes;
    }
    public override bool CanSubmit => _model.PopUpChosen != "";
    public override Task SubmitAsync()
    {
        return _processes.ChoseDealOrBuyAsync();
    }
}