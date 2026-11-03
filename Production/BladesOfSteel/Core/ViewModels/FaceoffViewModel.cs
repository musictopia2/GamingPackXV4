namespace BladesOfSteel.Core.ViewModels;
[InstanceGame]
public class FaceoffViewModel(BladesOfSteelVMData model, BladesOfSteelGameContainer gameContainer) : IPromptVM
{
    private async Task Deck1_DeckClickedAsync()
    {
        await gameContainer.DrawAsync!.Invoke();
    }
    void IPromptVM.Initialize()
    {
        model.Instructions = "Face-Off.  Click the deck to draw a card at random.  Whoever draws a higher number goes first for the game.  If there is a tie; then repeat.";
        model.Deck1.DeckClickedAsync = Deck1_DeckClickedAsync;
    }
}