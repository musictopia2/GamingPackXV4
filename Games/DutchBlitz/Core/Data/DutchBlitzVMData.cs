namespace DutchBlitz.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class DutchBlitzVMData : IBasicCardGamesData<DutchBlitzCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public DutchBlitzVMData(CommandContainer command, DutchBlitzGameContainer gameContainer)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        Stops = new CustomStopWatchCP();
        _command = command;
        _gameContainer = gameContainer;
        Stops.MaxTime = 7000;
        Pile1.Text = "Waste";
        StockPile = new StockViewModel(command);
        PublicPiles1 = new PublicViewModel(gameContainer);
        OtherPile = Pile1;
    }
    [LabelColumn]
    public string ErrorMessage { get; set; } = ""; //i think.
    public StockViewModel StockPile;
    public DiscardPilesVM<DutchBlitzCardInformation>? DiscardPiles;
    public PublicViewModel PublicPiles1;
    internal bool DidStartTimer { get; set; }
    internal void LoadDiscards()
    {
        if (_gameContainer!.PlayerList!.Count == 2)
        {
            _gameContainer.MaxDiscard = 5;
        }
        else if (_gameContainer.PlayerList!.Count == 3)
        {
            _gameContainer.MaxDiscard = 4;
        }
        else
        {
            _gameContainer.MaxDiscard = 3;
        }
        DiscardPiles = new DiscardPilesVM<DutchBlitzCardInformation>(_command);
        DiscardPiles.Init(_gameContainer.MaxDiscard);
    }
    public CustomStopWatchCP Stops;
    private readonly CommandContainer _command;
    private readonly DutchBlitzGameContainer _gameContainer;
    public DeckObservablePile<DutchBlitzCardInformation> Deck1 { get; set; }
    public SingleObservablePile<DutchBlitzCardInformation> Pile1 { get; set; }
    public HandObservable<DutchBlitzCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<DutchBlitzCardInformation>? OtherPile { get; set; }
}