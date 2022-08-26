namespace CarpetSolitaire.Core.Logic;
[SingletonGame]
public class CarpetSolitaireMainGameClass : SolitaireGameClass<CarpetSolitaireSaveInfo>
{
    public CarpetSolitaireMainGameClass(ISolitaireData solitaireData1,
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
        var aceList = GetAceList();
        AfterShuffle(aceList);
    }
    protected override CarpetSolitaireSaveInfo CloneSavedGame()
    {
        return SaveRoot.Clone();
    }
}