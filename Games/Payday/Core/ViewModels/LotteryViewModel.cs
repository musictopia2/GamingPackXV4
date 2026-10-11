namespace Payday.Core.ViewModels;
public class LotteryViewModel(CommandContainer commandContainer, PaydayVMData model, ILotteryProcesses processes) : BasicSubmitViewModel(commandContainer)
{
    public override bool CanSubmit => model.PopUpChosen != "";
    public override Task SubmitAsync()
    {
        return processes.ProcessLotteryAsync();
    }
}