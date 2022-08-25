namespace KlondikeSolitaire.Core.Logic;
[SingletonGame]
public class KlondikeSolitaireMainGameClass : SolitaireGameClass<KlondikeSolitaireSaveInfo>
{
    public KlondikeSolitaireMainGameClass(ISolitaireData solitaireData1,
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
        _thisMod!.MainPiles1!.ClearBoard();
        AfterShuffle();
    }
    protected override KlondikeSolitaireSaveInfo CloneSavedGame()
    {
        return SaveRoot.Clone();
    }
}