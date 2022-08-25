namespace EagleWingsSolitaire.Core.Logic;
[SingletonGame]
public class EagleWingsSolitaireMainGameClass : SolitaireGameClass<EagleWingsSolitaireSaveInfo>
{
    private readonly IToast _toast;
    public EagleWingsSolitaireMainGameClass(ISolitaireData solitaireData1,
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
    protected override SolitaireCard CardPlayed()
    {
        var thisCard = base.CardPlayed();
        if (thisCard.Deck > 0)
        {
            if (GlobalClass.MainModel!.Heel1.IsSelected)
            {
                return new SolitaireCard();
            }
            return thisCard;
        }
        thisCard = GlobalClass.MainModel!.Heel1!.DrawCard();
        thisCard.IsSelected = false;
        return thisCard;
    }
    protected override async Task<bool> HasOtherAsync(int pile)
    {
        if (GlobalClass.MainModel!.Heel1!.CardsLeft() != 1 || GlobalClass.MainModel.Heel1.IsSelected == false)
        {
            return await base.HasOtherAsync(pile);
        }
        int wastes = _thisMod!.WastePiles1!.OneSelected();
        if (wastes > -1)
        {
            _toast.ShowUserErrorToast("Can choose either the waste pile or from heel; but not both");
            return true;
        }
        _toast.ShowUserErrorToast("Cannot play from heel to wing since its the last card");
        return true;
    }
    public async Task HeelToMainAsync()
    {
        if (_thisMod!.WastePiles1!.OneSelected() > -1)
        {
            return;
        }
        int index = ValidMainColumn(GlobalClass.MainModel!.Heel1.RevealCard());
        if (index == -1)
        {
            GlobalClass.MainModel.Heel1.IsSelected = false;
            return;
        }
        var thisCard = GlobalClass.MainModel.Heel1.DrawCard();
        await FinishAddingToMainPileAsync(index, thisCard);
    }
    protected async override Task ContinueOpenSavedAsync()
    {
        var newList = SaveRoot.HeelList.GetNewObjectListFromDeckList(DeckList);
        GlobalClass.MainModel!.Heel1.OriginalList(newList);
        if (newList.Count == 1)
        {
            GlobalClass.MainModel.Heel1.DeckStyle = EnumDeckPileStyle.AlwaysKnown;
        }
        else
        {
            GlobalClass.MainModel.Heel1.DeckStyle = EnumDeckPileStyle.Unknown;
        }
        await base.ContinueOpenSavedAsync();
    }
    protected async override Task FinishSaveAsync()
    {
        SaveRoot.HeelList = GlobalClass.MainModel!.Heel1.GetCardIntegers();
        await base.FinishSaveAsync();
    }
    protected override void AfterShuffleCards()
    {
        var thisCol = CardList!.Take(13).ToRegularDeckDict();
        CardList!.RemoveRange(0, 13);
        GlobalClass.MainModel!.Heel1.DeckStyle = EnumDeckPileStyle.Unknown;
        GlobalClass.MainModel.Heel1.OriginalList(thisCol);
        thisCol = CardList.Take(1).ToRegularDeckDict();
        CardList.RemoveRange(0, 1);
        AfterShuffle(thisCol);
        CardList.Clear();
    }
    protected override EagleWingsSolitaireSaveInfo CloneSavedGame()
    {
        return SaveRoot.Clone();
    }
}