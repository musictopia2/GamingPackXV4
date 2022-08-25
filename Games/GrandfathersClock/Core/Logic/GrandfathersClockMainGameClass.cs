namespace GrandfathersClock.Core.Logic;
[SingletonGame]
public class GrandfathersClockMainGameClass : SolitaireGameClass<GrandfathersClockSaveInfo>
{
    public GrandfathersClockMainGameClass(ISolitaireData solitaireData1,
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
        DeckRegularDict<SolitaireCard> thisList = new()
        {
                FindCardBySuitValue(EnumRegularCardValueList.Ten, EnumSuitList.Hearts),
                FindCardBySuitValue(EnumRegularCardValueList.Jack, EnumSuitList.Spades),
                FindCardBySuitValue(EnumRegularCardValueList.Queen, EnumSuitList.Diamonds),
                FindCardBySuitValue(EnumRegularCardValueList.King, EnumSuitList.Clubs),
                FindCardBySuitValue(EnumRegularCardValueList.Two, EnumSuitList.Hearts),
                FindCardBySuitValue(EnumRegularCardValueList.Three, EnumSuitList.Spades),
                FindCardBySuitValue(EnumRegularCardValueList.Four, EnumSuitList.Diamonds),
                FindCardBySuitValue(EnumRegularCardValueList.Five, EnumSuitList.Clubs),
                FindCardBySuitValue(EnumRegularCardValueList.Six, EnumSuitList.Hearts),
                FindCardBySuitValue(EnumRegularCardValueList.Seven, EnumSuitList.Spades),
                FindCardBySuitValue(EnumRegularCardValueList.Eight, EnumSuitList.Diamonds),
                FindCardBySuitValue(EnumRegularCardValueList.Nine, EnumSuitList.Clubs)
            };
        CardList!.RemoveGivenList(thisList);
        AfterShuffle(thisList);
    }
    protected override GrandfathersClockSaveInfo CloneSavedGame()
    {
        return SaveRoot.Clone();
    }
}