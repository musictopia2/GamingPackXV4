namespace Payday.Core.ViewModels;
public class PlayerPickerViewModel(CommandContainer commandContainer, PaydayVMData model, IChoosePlayerProcesses processes) : BasicSubmitViewModel(commandContainer)
{
    public override bool CanSubmit => model.PopUpChosen != "";
    public override Task SubmitAsync()
    {
        return processes.ProcessChosenPlayerAsync();
    }
}