namespace Payday.Core.ViewModels;
public class BuyDealViewModel : BasicSubmitViewModel
{
    private readonly PaydayVMData _model;
    //private readonly IBuyProcesses _processes;
    public BuyDealViewModel(CommandContainer commandContainer, PaydayVMData model, IEventAggregator aggregator) : base(commandContainer, aggregator)
    {
        _model = model;
        //_processes = processes;
    }
    public override bool CanSubmit => _model.CurrentDealList.ObjectSelected() > 0;
    public override Task SubmitAsync()
    {
        //try to do nothing here.
        return Task.CompletedTask;
        //return _processes.BuyerSelectedAsync(_model.CurrentDealList.ObjectSelected());
    }
}