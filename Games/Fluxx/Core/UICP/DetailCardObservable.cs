namespace Fluxx.Core.UICP;
public class DetailCardObservable
{
    public FluxxCardInformation CurrentCard { get; set; }
    public void ResetCard()
    {
        CurrentCard = new();
        CurrentCard.IsUnknown = true;
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
    }
    public DetailCardObservable()
    {
        CurrentCard = new();
        CurrentCard.IsUnknown = true;
    }
}