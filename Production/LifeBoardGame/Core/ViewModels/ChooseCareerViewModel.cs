namespace LifeBoardGame.Core.ViewModels;
public class ChooseCareerViewModel(CommandContainer commandContainer,
    LifeBoardGameGameContainer gameContainer,
    LifeBoardGameVMData model,
    ICareerProcesses processes
        ) : BasicSubmitViewModel(commandContainer)
{
    public override bool CanSubmit
    {
        get
        {
            if (model.HandList.AutoSelect == EnumHandAutoType.SelectOneOnly)
            {
                return model.HandList.ObjectSelected() > 0;
            }
            if (gameContainer.SaveRoot.GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
            {
                if (gameContainer.SingleInfo!.OptionChosen != EnumStart.College)
                {
                    throw new CustomBasicException("Should have been one option alone.");
                }
                return model.HandList.HowManySelectedObjects == 3;
            }
            if (gameContainer.SaveRoot.GameStatus == EnumWhatStatus.NeedNewCareer)
            {
                return model.HandList.HowManySelectedObjects == 2;
            }
            throw new CustomBasicException("Cannot figure out whether it can submit or not.  Rethink");
        }
    }
    private void AdditionalCareerInstructions()
    {
        var tempList = model.HandList!.ListSelectedObjects();
        tempList.MakeAllObjectsKnown();
        model.HandList.HandList.ReplaceRange(tempList);
        model.HandList.Text = "Choose One Career";
        if (gameContainer.WasNight)
        {
            model.Instructions = "Choose one career or end turn and keep your current career";
        }
        model.HandList.AutoSelect = EnumHandAutoType.SelectOneOnly;
        model.HandList.UnselectAllObjects();
    }
    public override Task SubmitAsync()
    {
        if (model.HandList!.AutoSelect == EnumHandAutoType.SelectAsMany)
        {
            AdditionalCareerInstructions();
            return Task.CompletedTask;
        }
        if (gameContainer.GameStatus == EnumWhatStatus.NeedNewCareer || gameContainer.GameStatus == EnumWhatStatus.NeedChooseFirstCareer)
        {
            return processes.ChoseCareerAsync(model.HandList.ObjectSelected());
        }
        throw new CustomBasicException("Unable to submit based on the status of the game.  Rethink");
    }
}