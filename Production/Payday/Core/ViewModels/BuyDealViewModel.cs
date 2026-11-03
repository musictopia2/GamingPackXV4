namespace Payday.Core.ViewModels;
public class BuyDealViewModel(CommandContainer commandContainer, PaydayVMData model) : BasicSubmitViewModel(commandContainer)
{
    public override bool CanSubmit => model.CurrentDealList.ObjectSelected() > 0;
    public override Task SubmitAsync()
    {
        return Task.CompletedTask;
        //return _processes.BuyerSelectedAsync(_model.CurrentDealList.ObjectSelected());
    }
}