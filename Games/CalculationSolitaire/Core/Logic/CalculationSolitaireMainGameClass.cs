namespace CalculationSolitaire.Core.Logic;
[SingletonGame]
public class CalculationSolitaireMainGameClass(ISolitaireData solitaireData1,
    ISaveSinglePlayerClass thisState,
    IEventAggregator aggregator,
    IScoreData score,
    CommandContainer command,
    IToast toast,
    ISystemError error
        ) : SolitaireGameClass<CalculationSolitaireSaveInfo>(solitaireData1, thisState, aggregator, score, command, toast, error)
{
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
        DeckRegularDict<SolitaireCard> output = [];
        4.Times(x =>
        {
            var temps = CardList!.Where(items => items.Value.Value == x).ToRegularDeckDict();
            if (temps.Count != 4)
            {
                throw new CustomBasicException("There must be 4 cards");
            }
            var thisCard = temps.GetRandomItem();
            output.Add(thisCard);
            CardList!.RemoveSpecificItem(thisCard);
        });
        AfterShuffle(output);
        CardList!.Clear();
    }
    protected override CalculationSolitaireSaveInfo CloneSavedGame()
    {
        return SaveRoot.Clone();
    }
}