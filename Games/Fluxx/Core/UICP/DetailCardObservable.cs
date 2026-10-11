namespace Fluxx.Core.UICP;
public class DetailCardObservable
{
    public FluxxCardInformation CurrentCard { get; set; }
    public void ResetCard()
    {
        CurrentCard = new();
        CurrentCard.IsUnknown = true;
        CommandContainer command = Resolver!.Resolve<CommandContainer>();
        command.UpdateSpecificAction(FluxxActions.ShowCard);
    }
    public void ShowCard<F>(F thisCard)
        where F : FluxxCardInformation, new()
    {
        if (thisCard.Deck == CurrentCard.Deck)
        {
            return;
        }
        CurrentCard = FluxxDetailClass.GetNewCard(thisCard.Deck);
        CurrentCard.Populate(thisCard.Deck);
        CommandContainer command = Resolver!.Resolve<CommandContainer>();
        command.UpdateSpecificAction(FluxxActions.ShowCard);
    }
    public DetailCardObservable()
    {
        CurrentCard = new();
        CurrentCard.IsUnknown = true;
    }
}