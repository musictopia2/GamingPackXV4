namespace LifeCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class LifeCardGameGameContainer : CardGameContainer<LifeCardGameCardInformation, LifeCardGamePlayerItem, LifeCardGameSaveInfo>
{
    public LifeCardGameGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<LifeCardGameCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    internal DeckRegularDict<LifeCardGameCardInformation> YearCards() => DeckList.Where(items => items.CanBeInPlayerHandToBeginWith == false).ToRegularDeckDict();
    public LifeCardGameCardInformation? CardChosen { get; set; }
    public int PlayerChosen { get; set; }
    public DeckRegularDict<LifeCardGameCardInformation>? TradeList { get; set; } = new DeckRegularDict<LifeCardGameCardInformation>();
    internal Func<int, Task>? ChosePlayerAsync { get; set; }
    internal Func<Task>? LoadOtherScreenAsync { get; set; }
    internal Func<Task>? CloseOtherScreenAsync { get; set; }
    public LifeCardGamePlayerItem PlayerWithCard(LifeCardGameCardInformation thisCard)
    {
        var tempList = PlayerList!.ToBasicList();
        tempList.RemoveSpecificItem(SingleInfo!);
        foreach (var thisPlayer in tempList)
        {
            if (thisPlayer.LifeStory!.HandList.ObjectExist(thisCard.Deck))
            {
                return thisPlayer;
            }
        }
        throw new CustomBasicException("No player has the card");
    }
    public void CreateLifeStoryPile(LifeCardGameVMData model, LifeCardGamePlayerItem thisPlayer)
    {
        thisPlayer.LifeStory = new(this, model, thisPlayer.Id);
        thisPlayer.LifeStory.Text = thisPlayer.NickName;
    }
    public int OtherCardSelected()
    {
        var tempList = PlayerList!.AllPlayersExceptForCurrent();
        int decks = 0;
        int tempDeck;
        foreach (var thisPlayer in tempList)
        {
            tempDeck = thisPlayer.LifeStory!.ObjectSelected();
            if (tempDeck > 0)
            {
                if (decks > 0)
                {
                    return 0;
                }
                decks = tempDeck; //try here instead.
            }
            
        }
        return decks;
    }
}