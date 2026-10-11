namespace Fluxx.Core.ViewModels;
public class PlayViewModel(CommandContainer commandContainer,
    IPlayProcesses playProcesses,
    FluxxMainGameClass mainGame,
    FluxxGameContainer gameContainer,
    IToast toast,
    FluxxVMData model) : BasicSubmitViewModel(commandContainer)
{
    public override string Text => "Play";
    private bool IsOtherTurn => mainGame!.OtherTurn > 0;
    public override bool CanSubmit => !IsOtherTurn;
    public override async Task SubmitAsync()
    {
        if (model.Goal1!.ObjectSelected() > 0)
        {
            toast.ShowUserErrorToast("Cannot select any goal cards to play");
            model.UnselectAllCards();
            return;
        }
        if (model.Keeper1!.ObjectSelected() > 0)
        {
            toast.ShowUserErrorToast("Cannot select any keeper cards to play");
            model.UnselectAllCards();
            return;
        }
        if (gameContainer!.NeedsToRemoveGoal())
        {
            toast.ShowUserErrorToast("Cannot choose any cards to play until you discard a goal");
            model.UnselectAllCards();
            return;
        }
        int howMany = model.PlayerHand1!.HowManySelectedObjects;
        if (howMany != 1)
        {
            toast.ShowUserErrorToast("Must choose only one card to play");
            model.UnselectAllCards();
            return;
        }
        if (mainGame.SaveRoot!.PlaysLeft <= 0)
        {
            toast.ShowUserErrorToast("Sorry; you don't have any plays left");
            model.UnselectAllCards();
            return;
        }
        int deck = model.PlayerHand1.ObjectSelected();
        await playProcesses.SendPlayAsync(deck);
        await playProcesses.PlayCardAsync(deck);
    }
}