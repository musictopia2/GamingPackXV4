namespace BakersDozenSolitaire.Core.Logic;
[SingletonGame]
public class BakersDozenSolitaireMainGameClass : SolitaireGameClass<BakersDozenSolitaireSaveInfo>
{
    public BakersDozenSolitaireMainGameClass(ISolitaireData solitaireData1,
        ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IScoreData score,
        CommandContainer command,
        IToast toast,
        ISystemError error
        )
        : base(solitaireData1, thisState, aggregator, score, command, toast, error)
    {
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
        var firstList = DeckList.Where(items => items.Value == EnumRegularCardValueList.King).ToRegularDeckDict();
        DeckList.ReshuffleFirstObjects(firstList, 0, 12);
        CardList = DeckList.ToRegularDeckDict();
        _thisMod!.MainPiles1!.ClearBoard();
        AfterShuffle();
    }
    protected override BakersDozenSolitaireSaveInfo CloneSavedGame()
    {
        return SaveRoot.Clone();
    }
}