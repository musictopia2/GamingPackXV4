namespace PyramidSolitaire.Core.Logic;
public class PlayList : GameBoardObservable<SolitaireCard>
{
    public PlayList(CommandContainer container) : base(container)
    {
        Rows = 1;
        Columns = 2;
        HasFrame = true;
        Text = "Chosen Cards";
    }
    public bool AlreadyHasTwoCards() => ObjectList.Count == 2;
    public bool HasChosenCards() => ObjectList.Count != 0;
    public void RemoveOneCard(SolitaireCard thisCard) => ObjectList.RemoveObjectByDeck(thisCard.Deck);
    public void AddCard(SolitaireCard thisCard)
    {
        if (AlreadyHasTwoCards())
        {
            throw new CustomBasicException("Already has two cards.  Therefore, no cards can be added");
        }
        var newCard = new SolitaireCard();
        newCard.Populate(thisCard.Deck); //to clone.
        newCard.Visible = true; //to double check.
        ObjectList.Add(newCard);
    }
    public void RemoveCards() => ObjectList.Clear();
    protected override Task ClickProcessAsync(SolitaireCard card)
    {
        PyramidSolitaireMainGameClass game = aa.Resolver!.Resolve<PyramidSolitaireMainGameClass>();
        game.PutBack(card);
        return Task.CompletedTask;
    }
}