namespace Mastermind.Core.Data;
public class Guess
{
    public BasicList<Bead> YourBeads = new();
    public void GetNewBeads()
    {
        int x;
        BasicList<Bead> tempList = new();
        for (x = 1; x <= 4; x++)
        {
            Bead thisBead = new();
            thisBead.ColorChosen = EnumColorPossibilities.None;
            thisBead.CurrentGuess = this; // i think
            tempList.Add(thisBead);
        }
        YourBeads.ReplaceRange(tempList);
    }
    public bool IsCompleted { get; set; }
    public int HowManyBlacks { get; set; }
    public int HowManyAquas { get; set; }
}