namespace MonopolyCardGame.Core.ViewModels;
public class DetailCardViewModel
{
    public MonopolyCardGameCardInformation CurrentCard { get; set; }
    public void Clear()
    {
        if (CurrentCard.Deck == 0)
        {
            return;
        }
        CurrentCard = new ();
        CurrentCard.IsUnknown = true;
    }
    public void AdditionalInfo(int deck)
    {
        if (deck == CurrentCard.Deck)
        {
            return;
        }
        CurrentCard = new ();
        CurrentCard.Populate(deck);
    }
    public DetailCardViewModel()
    {
        CurrentCard = new ();
        CurrentCard.IsUnknown = true;
    }
}