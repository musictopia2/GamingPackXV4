namespace Payday.Core.ViewModels;
public class ChooseDealViewModel(CommandContainer commandContainer, PaydayVMData model, IDealProcesses processes) : BasicSubmitViewModel(commandContainer)
{
    public override bool CanSubmit => model.PopUpChosen != "";
    public override Task SubmitAsync()
    {
        return processes.ChoseWhetherToPurchaseDealAsync();
    }
}