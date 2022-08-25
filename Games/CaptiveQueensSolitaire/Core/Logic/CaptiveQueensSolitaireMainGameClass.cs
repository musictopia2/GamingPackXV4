namespace CaptiveQueensSolitaire.Core.Logic;
[SingletonGame]
public class CaptiveQueensSolitaireMainGameClass : SolitaireGameClass<CaptiveQueensSolitaireSaveInfo>
{
    public CaptiveQueensSolitaireMainGameClass(ISolitaireData solitaireData1,
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
        DeckRegularDict<SolitaireCard> output = new()
        {
            FindCardBySuitValue(EnumRegularCardValueList.Queen, EnumSuitList.Spades),
            FindCardBySuitValue(EnumRegularCardValueList.Queen, EnumSuitList.Diamonds),
            FindCardBySuitValue(EnumRegularCardValueList.Queen, EnumSuitList.Clubs),
            FindCardBySuitValue(EnumRegularCardValueList.Queen, EnumSuitList.Hearts)
        };
        CardList!.RemoveGivenList(output);
        output.Reverse();
        output.ForEach(thisCard => CardList.InsertBeginning(thisCard));
        _thisMod!.MainPiles1!.ClearBoard();
        AfterShuffle();
    }
    protected override CaptiveQueensSolitaireSaveInfo CloneSavedGame()
    {
        return SaveRoot.Clone();
    }
}