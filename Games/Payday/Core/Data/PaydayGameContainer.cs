namespace Payday.Core.Data;
[SingletonGame]
[AutoReset]
public class PaydayGameContainer : BasicGameContainer<PaydayPlayerItem, PaydaySaveInfo>
{
    public PaydayGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
    }
    public BasicList<GameSpace> PrivateSpaceList { get; set; } = new();
    public static CardInformation GetCard(int deck)
    {
        CardInformation output;
        if (deck <= 24)
        {
            output = new DealCard();
            output.Populate(deck);
            return output;
        }
        output = new MailCard();
        output.Populate(deck);
        return output;
    }
    public void RemoveOutCards(IDeckDict<CardInformation> listToRemove)
    {
        SaveRoot!.OutCards.RemoveGivenList(listToRemove); //hopefully this simple.
    }
    public void AddOutCard(CardInformation thisCard)
    {
        SaveRoot!.OutCards.Add(thisCard);
    }
    public int OtherTurn
    {
        get
        {
            return SaveRoot!.PlayOrder.OtherTurn;
        }
        set
        {
            SaveRoot!.PlayOrder.OtherTurn = value;
        }
    }
    public Func<int, Task>? SpaceClickedAsync { get; set; }
    public Func<int, Task>? ResultsOfMoveAsync { get; set; }
    public Func<Task>? OtherTurnProgressAsync { get; set; }
}