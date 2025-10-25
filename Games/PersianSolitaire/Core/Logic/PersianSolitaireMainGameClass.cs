namespace PersianSolitaire.Core.Logic;
[SingletonGame]
public partial class PersianSolitaireMainGameClass : SolitaireGameClass<PersianSolitaireSaveInfo>, IHandle<ScoreModel>
{
    readonly ScoreModel _model;
    public PersianSolitaireMainGameClass(ISolitaireData solitaireData1,
        ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IScoreData score,
        CommandContainer command,
        IToast toast,
        ISystemError error
        )
        : base(solitaireData1, thisState, aggregator, score, command, toast, error)
    {
        _model = (ScoreModel)score;
        aggregator.ClearSingle<ScoreModel>(); //just in case.
        Subscribe();
    }
    public void Close()
    {
        Unsubscribe();
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    protected async override Task ContinueOpenSavedAsync()
    {
        _model.DealNumber = SaveRoot.DealNumber;
        await base.ContinueOpenSavedAsync();
    }
    protected async override Task FinishSaveAsync()
    {
        if (SaveRoot.DealNumber == 0)
        {
            throw new CustomBasicException("The deal cannot be 0.  Rethink");
        }
        await base.FinishSaveAsync();
    }
    protected override SolitaireCard CardPlayed()
    {
        var thisCard = base.CardPlayed();
        return thisCard;
    }
    private PersianSolitaireMainViewModel? _newVM;
    protected override void AfterShuffleCards()
    {
        _model.DealNumber = 1;
        _thisMod!.MainPiles1!.ClearBoard();
        AfterShuffle();
    }
    void IHandle<ScoreModel>.Handle(ScoreModel message)
    {
        if (_newVM == null)
        {
            _newVM = (PersianSolitaireMainViewModel)_thisMod!;
        }
        _newVM.DealNumber = message.DealNumber;
        SolitairePilesCP.DealNumber = message.DealNumber;
        SaveRoot.DealNumber = message.DealNumber; //hopefully this simple.
    }
    protected override PersianSolitaireSaveInfo CloneSavedGame()
    {
        return SaveRoot.Clone();
    }
}