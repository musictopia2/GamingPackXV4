namespace Payday.Core.ViewModels;
public class LotteryViewModel : BasicSubmitViewModel
{
    private readonly PaydayVMData _model;
    private readonly ILotteryProcesses _processes;
    public LotteryViewModel(CommandContainer commandContainer, PaydayVMData model, ILotteryProcesses processes, IEventAggregator aggregator) : base(commandContainer, aggregator)
    {
        _model = model;
        _processes = processes;
    }
    public override bool CanSubmit => _model.PopUpChosen != "";
    public override Task SubmitAsync()
    {
        return _processes.ProcessLotteryAsync();
    }
}