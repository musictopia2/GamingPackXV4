namespace Mastermind.Core.Data;
[UseStateChanged]
public partial class Guess
{
    public BasicList<Bead> YourBeads = [];
    public void GetNewBeads()
    {
        int x;
        BasicList<Bead> tempList = [];
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
    [StateProperty]
    public partial int HowManyBlacks { get; set; }
    [StateProperty]
    public partial int HowManyAquas { get; set; }
}