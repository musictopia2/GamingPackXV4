namespace Payday.Core.Data;
[SingletonGame]
public class PaydaySaveInfo : BasicSavedBoardDiceGameClass<PaydayPlayerItem>, IMappable, ISaveInfo
{
    private IEventAggregator? _aggregator;
    private string _instructions = "";
    public string Instructions
    {
        get { return _instructions; }
        set
        {
            if (SetProperty(ref _instructions, value))
            {
                if (_model != null)
                {
                    _model.Instructions = value;
                }
            }
        }
    }
    private EnumStatus _gameStatus;
    public EnumStatus GameStatus
    {
        get { return _gameStatus; }
        set
        {
            if (SetProperty(ref _gameStatus, value))
            {
                if (_model != null)
                {
                    _model.GameStatus = value;
                    if (_aggregator is null)
                    {
                        _aggregator = aa1.Resolver!.Resolve<IEventAggregator>();
                    }
                    _aggregator.Publish(GameStatus);
                }
            }
        }
    }
    private PaydayVMData? _model;
    internal void LoadMod(PaydayVMData model)
    {
        _model = model;
        _model.Instructions = Instructions;
        _model.GameStatus = GameStatus;
    }
    public int NumberHighlighted { get; set; }
    public decimal LotteryAmount { get; set; }
    public int MaxMonths { get; set; }
    public DealCard? YardSaleDealCard { get; set; }
    public DeckRegularDict<MailCard> MailListLeft { get; set; } = new();
    public DeckRegularDict<DealCard> DealListLeft { get; set; } = new();
    public DeckRegularDict<CardInformation> OutCards { get; set; } = new();
    public int RemainingMove { get; set; }
    public bool EndOfMonth { get; set; }
    public bool EndGame { get; set; }
    public int DiceNumber { get; set; }
    public MailCard CurrentMail { get; set; } = new();
    public DealCard? CurrentDeal { get; set; }
}