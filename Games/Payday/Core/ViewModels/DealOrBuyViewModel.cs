namespace Payday.Core.ViewModels;
public class DealOrBuyViewModel(CommandContainer commandContainer, PaydayVMData model, IDealBuyChoiceProcesses processes) : BasicSubmitViewModel(commandContainer)
{
    public override bool CanSubmit => model.PopUpChosen != "";
    public override Task SubmitAsync()
    {
        return processes.ChoseDealOrBuyAsync();
    }
}