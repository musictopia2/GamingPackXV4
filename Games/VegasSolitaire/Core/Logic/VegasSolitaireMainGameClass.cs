namespace VegasSolitaire.Core.Logic;
[SingletonGame]
public class VegasSolitaireMainGameClass : SolitaireGameClass<VegasSolitaireSaveInfo>
{
    public VegasSolitaireMainGameClass(ISolitaireData solitaireData1,
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
    internal Action? AddMoney { get; set; }
    internal Action? ResetMoney { get; set; }
    protected override void AddToScore()
    {
        AddMoney!.Invoke();
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
        ResetMoney!.Invoke();
        AfterShuffle();
    }
    protected override VegasSolitaireSaveInfo CloneSavedGame()
    {
        return SaveRoot.Clone();
    }
}