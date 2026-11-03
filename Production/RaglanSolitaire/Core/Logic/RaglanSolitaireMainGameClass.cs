namespace RaglanSolitaire.Core.Logic;
[SingletonGame]
public class RaglanSolitaireMainGameClass(ISolitaireData solitaireData1,
    ISaveSinglePlayerClass thisState,
    IEventAggregator aggregator,
    IScoreData score,
    CommandContainer command,
    IToast toast,
    ISystemError error
        ) : SolitaireGameClass<RaglanSolitaireSaveInfo>(solitaireData1, thisState, aggregator, score, command, toast, error)
{
    protected override async Task<bool> HasOtherAsync(int pile)
    {
        if (GlobalClass.Stock!.ObjectSelected() == 0)
        {
            return await base.HasOtherAsync(pile);
        }
        int wastes = _thisMod!.WastePiles1!.OneSelected();
        if (wastes > -1)
        {
            Toast.ShowUserErrorToast("Cannot choose both from the waste and the stock");
            return true;
        }
        var thisCard = GlobalClass.Stock.HandList.GetSpecificItem(GlobalClass.Stock.ObjectSelected());
        if (_thisMod.WastePiles1.CanAddSingleCard(pile, thisCard) == false)
        {
            Toast.ShowUserErrorToast("Illegal move");
            return true;
        }
        _thisMod.WastePiles1.AddSingleCard(pile, thisCard);
        GlobalClass.Stock.HandList.RemoveObjectByDeck(thisCard.Deck);
        UpdateWastePiles();
        return true;
    }
    protected override void RemoveFromMiscPiles(SolitaireCard thisCard)
    {
        GlobalClass.Stock!.HandList.RemoveObjectByDeck(thisCard.Deck);
    }
    protected override SolitaireCard CardPlayed()
    {
        var thisCard = base.CardPlayed();
        if (thisCard.Deck > 0)
        {
            if (GlobalClass.Stock!.ObjectSelected() > 0)
            {
                return new SolitaireCard();
            }
            return thisCard;
        }
        thisCard = GlobalClass.Stock!.HandList.GetSpecificItem(GlobalClass.Stock.ObjectSelected());
        return thisCard;
    }
    protected override void AfterShuffleCards()
    {
        var aceList = GetAceList();
        AfterShuffle(aceList);
    }
    protected override void PopulateDeck(IEnumerableDeck<SolitaireCard> leftOverList)
    {
        GlobalClass.Stock!.HandList.ReplaceRange(leftOverList);
    }
    protected async override Task ContinueOpenSavedAsync()
    {
        GlobalClass.Stock!.HandList.ReplaceRange(SaveRoot.StockCards);
        await base.ContinueOpenSavedAsync();
    }
    protected async override Task FinishSaveAsync()
    {
        SaveRoot.StockCards = GlobalClass.Stock!.HandList.ToRegularDeckDict();
        await base.FinishSaveAsync();
    }
    protected override RaglanSolitaireSaveInfo CloneSavedGame()
    {
        return SaveRoot.Clone();
    }
}