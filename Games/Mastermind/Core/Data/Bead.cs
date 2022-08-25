namespace Mastermind.Core.Data;
public class Bead
{
    public EnumColorPossibilities ColorChosen { get; set; } = EnumColorPossibilities.None;
    public bool IsEnabled { get; set; }
    public bool DidCheck { get; set; }
    public Guess? CurrentGuess; // hopefully no binding.  however, this is needed so when i get the bead, i have the guess involved for this.
    public Bead() { } //needed so there is a choice now.
    public Bead(EnumColorPossibilities color)
    {
        ColorChosen = color;
    }
}