namespace FreeCellSolitaire.Core.Logic;
[SingletonGame]
public class FreeCellSolitaireMainGameClass : SolitaireGameClass<FreeCellSolitaireSaveInfo>
{
    private WastePiles? _thisWaste;
    private FreePiles? _freepile;
    private readonly IToast _toast;
    public FreeCellSolitaireMainGameClass(ISolitaireData solitaireData1,
        ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IScoreData score,
        CommandContainer command,
        IToast toast,
        ISystemError error
        )
        : base(solitaireData1, thisState, aggregator, score, command, toast, error)
    {
        _toast = toast;
    }
    public override Task NewGameAsync()
    {
        if (_thisWaste == null)
        {
            _thisWaste = (WastePiles)_thisMod!.WastePiles1!;
        }
        if (_freepile == null)
        {
            var model = (FreeCellSolitaireMainViewModel)_thisMod!;
            _freepile = model.FreePiles1;
        }
        return base.NewGameAsync();
    }
    public void FreeSelected(int pile)
    {
        if (_thisMod!.WastePiles1!.OneSelected() == -1)
        {
            if (_freepile!.OneSelected == pile)
            {
                _freepile.SelectUnselectCard(pile);
                return;
            }
            if (_freepile.OneSelected > -1)
            {
                _toast.ShowUserErrorToast("Illegal move");
                return;
            }
            if (_freepile.HasCard(pile) == false)
            {
                _toast.ShowUserErrorToast("Illegal move");
                return;
            }
            _freepile.SelectUnselectCard(pile);
            return;
        }
        if (_freepile!.HasCard(pile))
        {
            _toast.ShowUserErrorToast("Illegal move");
            return;
        }
        var thisCard = _thisMod.WastePiles1.GetCard();
        _freepile.AddCardToPile(pile, thisCard);
        _thisMod.WastePiles1.RemoveSingleCard();
    }
    private int HowMuchToEmpty()
    {
        int freePiles = _thisWaste!.FreePiles;
        int freeCells = _freepile!.HowManyFreeCells;
        if (freeCells == 0 && freePiles == 0)
        {
            throw new CustomBasicException("There was no free piles or free cells");
        }
        if (freeCells == 1 && freePiles == 0)
        {
            return 1;
        }
        return freeCells + 1;
    }
    private int HowMuchRoom()
    {
        int freePiles;
        int freeCells;
        freePiles = _thisWaste!.FreePiles;
        freeCells = _freepile!.HowManyFreeCells;
        if (freePiles == 0 && freeCells == 0)
        {
            return 1;
        }
        if (freePiles == 0)
        {
            return freeCells + 1;
        }
        if ((freePiles == 1) & (freeCells == 0))
        {
            return 2;
        }
        if ((freePiles == 1) & (freeCells == 0))
        {
            return 2;
        }
        if ((freePiles == 1) & (freeCells == 1))
        {
            return 4;
        }
        if ((freeCells == 2) & (freePiles == 1))
        {
            return 6;
        }
        if ((freeCells == 3) & (freePiles == 1))
        {
            return 8;
        }
        if ((freeCells == 4) & (freePiles == 1))
        {
            return 10;
        }
        if (freePiles == 2 && freeCells == 1)
        {
            return 6;
        }
        if (freePiles == 2 && freeCells == 2)
        {
            return 9;
        }
        if (freePiles == 2 && freeCells == 3)
        {
            return 12;
        }
        if (freeCells == 2 && freeCells == 4)
        {
            return 13;
        }
        if (freePiles == 3 && freeCells == 1)
        {
            return 8;
        }
        if (freePiles == 3 && freeCells == 2)
        {
            return 10;
        }
        if (freePiles == 3 && freeCells == 3)
        {
            return 12;
        }
        if (freePiles == 4 && freeCells == 1)
        {
            return 10;
        }
        return 13;
    }
    protected override async Task<bool> HasOtherAsync(int pile)
    {
        await Task.Delay(0);
        if (_thisMod!.WastePiles1!.OneSelected() == pile)
        {
            _thisMod.WastePiles1.SelectUnselectPile(pile);
            return true;
        }
        if (_thisMod.WastePiles1.OneSelected() == -1 && _freepile!.OneSelected == -1)
        {
            if (_thisMod.WastePiles1.HasCard(pile) == false)
            {
                return true;
            }
            _thisMod.WastePiles1.SelectUnselectPile(pile);
            return true;
        }
        if (_freepile!.OneSelected > -1)
        {
            var thisCard = _freepile.GetCard();
            if (_thisMod.WastePiles1.CanAddSingleCard(pile, thisCard) == false)
            {
                _toast.ShowUserErrorToast("Illegal move");
                return true;
            }
            _thisMod.WastePiles1.AddSingleCard(pile, thisCard);
            _freepile.RemoveCard();
            return true;
        }
        int rooms;
        if (_thisMod.WastePiles1.HasCard(pile) == false)
        {
            rooms = HowMuchToEmpty();
        }
        else
        {
            rooms = HowMuchRoom();
        }
        if (_thisWaste!.CanMoveCards(pile, out int lasts, rooms) == false)
        {
            _toast.ShowUserErrorToast("Illegal move");
            return true;
        }
        _thisMod.WastePiles1.MoveCards(pile, lasts);
        return true;
    }
    protected override SolitaireCard CardPlayed()
    {
        var thisCard = base.CardPlayed();
        if (thisCard.Deck > 0)
        {
            return thisCard;
        }
        int ones = _freepile!.OneSelected;
        if (ones == -1)
        {
            return new SolitaireCard();
        }
        return _freepile.GetCard();
    }
    protected override void AfterShuffleCards()
    {
        _freepile!.ClearBoard();
        _thisMod!.MainPiles1!.ClearBoard();
        AfterShuffle();
    }
    protected override void RemoveFromMiscPiles(SolitaireCard thisCard)
    {
        _freepile!.RemoveCard();
    }
    protected async override Task ContinueOpenSavedAsync()
    {
        _freepile!.PileList = SaveRoot.FreeCards.ToBasicList();
        await base.ContinueOpenSavedAsync();
    }
    protected async override Task FinishSaveAsync()
    {
        SaveRoot.FreeCards = _freepile!.PileList!.ToBasicList();
        await base.FinishSaveAsync();
    }
    protected override FreeCellSolitaireSaveInfo CloneSavedGame()
    {
        return SaveRoot.Clone();
    }
}