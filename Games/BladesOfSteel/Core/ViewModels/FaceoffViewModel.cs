namespace BladesOfSteel.Core.ViewModels;
[InstanceGame]
public class FaceoffViewModel : ScreenViewModel, IBlankGameVM
{
    private readonly BladesOfSteelVMData _model;
    private readonly BladesOfSteelGameContainer _gameContainer;
    public FaceoffViewModel(CommandContainer commandContainer, BladesOfSteelVMData model, BladesOfSteelGameContainer gameContainer, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _model = model;
        _model.Instructions = "Face-Off.  Click the deck to draw a card at random.  Whoever draws a higher number goes first for the game.  If there is a tie; then repeat.";
        _gameContainer = gameContainer;
        _model.Deck1.DeckClickedAsync = Deck1_DeckClickedAsync;
    }
    private async Task Deck1_DeckClickedAsync()
    {
        await _gameContainer.DrawAsync!.Invoke();
    }
    public CommandContainer CommandContainer { get; set; }
}