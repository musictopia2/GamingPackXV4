namespace Mastermind.Blazor.Views;
public partial class HintUI
{
    [Parameter]
    public Guess? YourGuess { get; set; }
    private int LeftOvers => 4 - YourGuess!.HowManyAquas - YourGuess.HowManyBlacks;
}