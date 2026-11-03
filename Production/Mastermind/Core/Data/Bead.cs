namespace Mastermind.Core.Data;
[UseStateChanged]
public partial class Bead
{
    [StateProperty]
    public partial EnumColorPossibilities ColorChosen { get; set; }
    public bool IsEnabled { get; set; }
    public bool DidCheck { get; set; }
    public Guess? CurrentGuess; // hopefully no binding.  however, this is needed so when i get the bead, i have the guess involved for this.
    public Bead() { } //needed so there is a choice now.
    public Bead(EnumColorPossibilities color)
    {
        ColorChosen = color;
    }
}