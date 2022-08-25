namespace EightOffSolitaire.Core.Logic;
[SingletonGame]
public class EightOffSolitaireMainGameClass : SolitaireGameClass<EightOffSolitaireSaveInfo>
{
    private readonly IToast _toast;
    private readonly IMessageBox _message;
    public EightOffSolitaireMainGameClass(ISolitaireData solitaireData1,
        ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IScoreData score,
        CommandContainer command,
        IToast toast,
        ISystemError error,
        IMessageBox message
        )
        : base(solitaireData1, thisState, aggregator, score, command, toast, error)
    {
        _toast = toast;
        _message = message;
    }
    protected async override Task ContinueOpenSavedAsync()
    {
        GlobalClass.MainModel!.ReservePiles1.HandList.ReplaceRange(SaveRoot.ReserveList);
        await base.ContinueOpenSavedAsync();
    }
    protected async override Task FinishSaveAsync()
    {
        SaveRoot.ReserveList = GlobalClass.MainModel!.ReservePiles1.HandList.ToRegularDeckDict();
        await base.FinishSaveAsync();
    }
    public override Task NewGameAsync()
    {
        GlobalClass.MainModel!.ReservePiles1!.ClearBoard();
        return base.NewGameAsync();
    }
    protected override SolitaireCard CardPlayed()
    {
        var thisCard = base.CardPlayed();
        if (thisCard.Deck > 0)
        {
            if (GlobalClass.MainModel!.ReservePiles1!.ObjectSelected() > 0)
            {
                return new SolitaireCard();
            }
            return thisCard;
        }
        return GlobalClass.MainModel!.ReservePiles1!.GetCardSelected();
    }
    protected override void RemoveFromMiscPiles(SolitaireCard thisCard)
    {
        GlobalClass.MainModel!.ReservePiles1!.RemoveCard(thisCard);
    }

    //for now, no double click.  if we decide to support it, then will be here.
    protected override async Task<bool> HasOtherAsync(int pile)
    {
        int reserves = GlobalClass.MainModel!.ReservePiles1!.ObjectSelected();
        int wastes = _thisMod!.WastePiles1!.OneSelected();
        if (reserves > 0 && wastes > -1)
        {
            _toast.ShowUserErrorToast("Must either select a reserve or a waste pile but not both");
            return true;
        }
        if (wastes == pile || wastes == -1 && reserves == 0)
        {
            _thisMod.WastePiles1.SelectUnselectPile(pile); //i think.
            return true;
        }
        if (reserves > 0)
        {
            if (_thisMod.WastePiles1.CanAddSingleCard(pile, GlobalClass.MainModel.ReservePiles1.GetCardSelected()) == false)
            {
                await _message.ShowMessageAsync("Illegal Move");
                return true;
            }
            var oldCard = GlobalClass.MainModel!.ReservePiles1.GetCardSelected();
            GlobalClass.MainModel!.ReservePiles1.RemoveCard(oldCard);
            _thisMod.WastePiles1.AddSingleCard(pile, oldCard);
            return true;
        }
        var thisCard = _thisMod.WastePiles1.GetCard();
        if (_thisMod.WastePiles1.CanAddSingleCard(pile, thisCard) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            return true;
        }
        _thisMod.WastePiles1.MoveSingleCard(pile);
        return true;
    }
    public void AddToReserve()
    {
        if (GlobalClass.MainModel!.ReservePiles1!.HowManyCards >= 8)
        {
            _toast.ShowUserErrorToast("There can only be 8 cards to reserve.  Therefore, cannot add any more cards to reserve");
            return;
        }
        if (GlobalClass.MainModel.ReservePiles1.ObjectSelected() > 0)
        {
            _toast.ShowUserErrorToast("There is already a card selected.  Unselect the card first before adding a card to reserve");
            return;
        }
        if (_thisMod!.WastePiles1!.OneSelected() == -1)
        {
            _toast.ShowUserErrorToast("There is no card selected to add to reserve");
            return;
        }
        var thisCard = _thisMod.WastePiles1.GetCard();
        GlobalClass.MainModel!.ReservePiles1.AddCard(thisCard);
        _thisMod.WastePiles1.RemoveSingleCard();
    }
    protected override void AfterShuffleCards()
    {
        var aceList = GetAceList();
        AfterShuffle(aceList);
    }
    protected override EightOffSolitaireSaveInfo CloneSavedGame()
    {
        return SaveRoot.Clone();
    }
}