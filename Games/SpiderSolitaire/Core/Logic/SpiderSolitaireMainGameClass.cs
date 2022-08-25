namespace SpiderSolitaire.Core.Logic;
[SingletonGame]
public class SpiderSolitaireMainGameClass : SolitaireGameClass<SpiderSolitaireSaveInfo>
{
    private readonly IScoreData _score;
    public SpiderSolitaireMainGameClass(ISolitaireData solitaireData1,
        ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IScoreData score,
        CommandContainer command,
        IToast toast,
        ISystemError error
        )
        : base(solitaireData1, thisState, aggregator, score, command, toast, error)
    {
        _score = score;
    }
    private WastePiles? _tempWaste;
    public override Task NewGameAsync()
    {
        _tempWaste = (WastePiles)_thisMod!.WastePiles1;
        return base.NewGameAsync();
    }
    protected async override Task ContinueOpenSavedAsync()
    {
        //anything else you need will be here
        await base.ContinueOpenSavedAsync();
    }
    protected async override Task FinishSaveAsync()
    {
        //anything else to finish save will be here.
        await base.FinishSaveAsync();
    }
    protected override SolitaireCard CardPlayed()
    {
        var thisCard = base.CardPlayed();
        return thisCard;
        //if any changes, will be here.
    }
    protected override void AfterShuffleCards()
    {
        _thisMod!.MainPiles1!.ClearBoard();
        AfterShuffle();
    }
    public override void DrawCard()
    {
        CardList = _thisMod!.DeckPile!.DrawSeveralCards(10, false);
        _tempWaste!.AddCards(CardList);
        10.Times(x =>
        {
            if (_tempWaste.CanMoveAll(x - 1))
            {
                MoveCards(x - 1);
            }
        });
    }
    private void MoveCards(int whichOne)
    {
        var tempList = _tempWaste!.MoveList();
        if (tempList.Count != 13)
        {
            throw new CustomBasicException("Must have 13 cards.  Otherwise, can't move the cards");
        }
        _thisMod!.MainPiles1!.AddCards(tempList);
        _tempWaste.RemoveCards(whichOne);
    }
    public override async Task AfterMovingCardsAsync(int whichOne)
    {
        if (_tempWaste!.CanMoveAll(whichOne) == false)
        {
            await base.AfterMovingCardsAsync(whichOne);
            return;
        }
        MoveCards(whichOne);
        if (_score.Score == HowManyCards)
        {
            await ShowWinAsync();
            return;
        }
        await base.AfterMovingCardsAsync(whichOne);
    }
    protected override SpiderSolitaireSaveInfo CloneSavedGame()
    {
        return SaveRoot.Clone();
    }
}