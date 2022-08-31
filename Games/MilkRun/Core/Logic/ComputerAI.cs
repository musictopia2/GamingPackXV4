namespace MilkRun.Core.Data;
[SingletonGame]
[AutoReset]
public class ComputerAI
{
    public struct MoveInfo
    {
        public int Player;
        public bool ToDiscard;
        public EnumMilkType Milk;
        public EnumPileType Pile;
        public int Deck;
    }
    private readonly MilkRunGameContainer _gameContainer;
    public ComputerAI(MilkRunGameContainer gameContainer)
    {
        _gameContainer = gameContainer;
    }
    private BasicList<MoveInfo> MoveList(int deck)
    {
        BasicList<MoveInfo> output = new();
        int selfPlayer = _gameContainer.PlayerList!.GetSelf().Id;
        int computerPlayer;
        MoveInfo thisMove;
        if (_gameContainer.CanMakeMove == null)
        {
            throw new CustomBasicException("Nobody is handling canmakemove.  Rethink");
        }
        if (selfPlayer == 1)
        {
            computerPlayer = 2;
        }
        else
        {
            computerPlayer = 1;
        }
        if (_gameContainer.CanMakeMove(computerPlayer, deck, EnumPileType.Deliveries, EnumMilkType.Chocolate))
        {
            thisMove = new ();
            thisMove.Deck = deck;
            thisMove.Player = computerPlayer;
            thisMove.Milk = EnumMilkType.Chocolate;
            thisMove.Pile = EnumPileType.Deliveries;
            output.Add(thisMove);
        }
        if (_gameContainer.CanMakeMove(computerPlayer, deck, EnumPileType.Go, EnumMilkType.Chocolate))
        {
            thisMove = new ();
            thisMove.Deck = deck;
            thisMove.Player = computerPlayer;
            thisMove.Milk = EnumMilkType.Chocolate;
            thisMove.Pile = EnumPileType.Go;
            output.Add(thisMove);
        }
        if (_gameContainer.CanMakeMove(computerPlayer, deck, EnumPileType.Limit, EnumMilkType.Chocolate))
        {
            thisMove = new ();
            thisMove.Deck = deck;
            thisMove.Player = computerPlayer;
            thisMove.Milk = EnumMilkType.Chocolate;
            thisMove.Pile = EnumPileType.Limit;
            output.Add(thisMove);
        }
        if (_gameContainer.CanMakeMove(computerPlayer, deck, EnumPileType.Deliveries, EnumMilkType.Strawberry))
        {
            thisMove = new ();
            thisMove.Deck = deck;
            thisMove.Player = computerPlayer;
            thisMove.Milk = EnumMilkType.Strawberry;
            thisMove.Pile = EnumPileType.Deliveries;
            output.Add(thisMove);
        }
        if (_gameContainer.CanMakeMove(computerPlayer, deck, EnumPileType.Go, EnumMilkType.Strawberry))
        {
            thisMove = new ();
            thisMove.Deck = deck;
            thisMove.Player = computerPlayer;
            thisMove.Milk = EnumMilkType.Strawberry;
            thisMove.Pile = EnumPileType.Go;
            output.Add(thisMove);
        }
        if (_gameContainer.CanMakeMove(computerPlayer, deck, EnumPileType.Limit, EnumMilkType.Strawberry))
        {
            thisMove = new ();
            thisMove.Deck = deck;
            thisMove.Player = computerPlayer;
            thisMove.Milk = EnumMilkType.Strawberry;
            thisMove.Pile = EnumPileType.Limit;
            output.Add(thisMove);
        }
        //human player;
        if (_gameContainer.CanMakeMove(selfPlayer, deck, EnumPileType.Deliveries, EnumMilkType.Chocolate))
        {
            thisMove = new ();
            thisMove.Deck = deck;
            thisMove.Player = selfPlayer;
            thisMove.Milk = EnumMilkType.Chocolate;
            thisMove.Pile = EnumPileType.Deliveries;
            output.Add(thisMove);
        }
        if (_gameContainer.CanMakeMove(selfPlayer, deck, EnumPileType.Go, EnumMilkType.Chocolate))
        {
            thisMove = new ();
            thisMove.Deck = deck;
            thisMove.Player = selfPlayer;
            thisMove.Milk = EnumMilkType.Chocolate;
            thisMove.Pile = EnumPileType.Go;
            output.Add(thisMove);
        }
        if (_gameContainer.CanMakeMove(selfPlayer, deck, EnumPileType.Limit, EnumMilkType.Chocolate))
        {
            thisMove = new ();
            thisMove.Deck = deck;
            thisMove.Player = selfPlayer;
            thisMove.Milk = EnumMilkType.Chocolate;
            thisMove.Pile = EnumPileType.Limit;
            output.Add(thisMove);
        }
        if (_gameContainer.CanMakeMove(selfPlayer, deck, EnumPileType.Deliveries, EnumMilkType.Strawberry))
        {
            thisMove = new ();
            thisMove.Deck = deck;
            thisMove.Player = selfPlayer;
            thisMove.Milk = EnumMilkType.Strawberry;
            thisMove.Pile = EnumPileType.Deliveries;
            output.Add(thisMove);
        }
        if (_gameContainer.CanMakeMove(selfPlayer, deck, EnumPileType.Go, EnumMilkType.Strawberry))
        {
            thisMove = new ();
            thisMove.Deck = deck;
            thisMove.Player = selfPlayer;
            thisMove.Milk = EnumMilkType.Strawberry;
            thisMove.Pile = EnumPileType.Go;
            output.Add(thisMove);
        }
        if (_gameContainer.CanMakeMove(selfPlayer, deck, EnumPileType.Limit, EnumMilkType.Strawberry))
        {
            thisMove = new ();
            thisMove.Deck = deck;
            thisMove.Player = selfPlayer;
            thisMove.Milk = EnumMilkType.Strawberry;
            thisMove.Pile = EnumPileType.Limit;
            output.Add(thisMove);
        }
        return output;
    }
    public MoveInfo MoveToMake()
    {
        BasicList<MoveInfo> newList = new();
        _gameContainer.SingleInfo!.MainHandList.ForEach(thisCard =>
        {
            newList.AddRange(MoveList(thisCard.Deck));
            MoveInfo thisMove = new();
            thisMove.ToDiscard = true;
            thisMove.Deck = thisCard.Deck;
            newList.Add(thisMove);
        });
        return newList.GetRandomItem();
    }
    public bool CanDraw => _gameContainer.SaveRoot!.CardsDrawn < 2;
}
