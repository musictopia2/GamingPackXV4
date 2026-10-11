namespace Fluxx.Core.ViewModels;
public class GiveViewModel(
    CommandContainer commandContainer,
    FluxxVMData model,
    FluxxGameContainer gameContainer,
    IGiveTaxationProcesses processes,
    IToast toast
        ) : BasicSubmitViewModel(commandContainer)
{
    public override string Text => "Give";
    public override bool CanSubmit => true;
    public override async Task SubmitAsync()
    {
        if (model.Keeper1!.HowManySelectedObjects > 0)
        {
            toast.ShowUserErrorToast("Cannot select any keeper cards because you have to give the cards from your hand");
            model.UnselectAllCards();
            return;
        }
        if (model.Goal1!.ObjectSelected() > 0)
        {
            toast.ShowUserErrorToast("Cannot select any goal cards because you have to give the cards from your hand");
            model.UnselectAllCards();
            return;
        }
        int howMany = gameContainer.IncreaseAmount() + 1;
        if (model.PlayerHand1!.HowManySelectedObjects == howMany || model.PlayerHand1.HowManySelectedObjects == model.PlayerHand1.HandList.Count)
        {
            var thisList = model.PlayerHand1.ListSelectedObjects(true);
            await processes.GiveCardsForTaxationAsync(thisList);
            return;
        }
        if (howMany > model.PlayerHand1.HandList.Count)
        {
            howMany = model.PlayerHand1.HandList.Count;
        }
        toast.ShowUserErrorToast($"Must give {howMany} not {model.PlayerHand1.HowManySelectedObjects} cards");
        model.UnselectAllCards();
    }
}