namespace HuseHearts.Core.ViewModels;
public class PassingViewModel(CommandContainer commandContainer,
    HuseHeartsMainGameClass mainGame,
    HuseHeartsVMData model,
    IToast toast
        ) : BasicSubmitViewModel(commandContainer)
{
    public override string Text => "Pass Cards";

    public override bool CanSubmit
    {
        get
        {
            return true;
        }
    }
    public override async Task SubmitAsync()
    {
        int cardsSelected = model.PlayerHand1!.HowManySelectedObjects;
        if (cardsSelected != 3)
        {
            toast.ShowUserErrorToast("Must pass 3 cards");
            return;
        }
        var tempList = model.PlayerHand1.ListSelectedObjects().GetDeckListFromObjectList();
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("passcards", tempList);
        }
        await mainGame!.CardsPassedAsync(tempList);
    }
}
