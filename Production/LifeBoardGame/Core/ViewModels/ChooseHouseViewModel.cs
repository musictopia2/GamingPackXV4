namespace LifeBoardGame.Core.ViewModels;
public class ChooseHouseViewModel(CommandContainer commandContainer,
    LifeBoardGameVMData model,
    IHouseProcesses processes
        ) : BasicSubmitViewModel(commandContainer)
{
    public override bool CanSubmit => model.HandList.ObjectSelected() > 0;
    public override Task SubmitAsync()
    {
        return processes.ChoseHouseAsync(model.HandList.ObjectSelected());
    }
}