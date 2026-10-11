namespace Mastermind.Core.Logic;
[SingletonGame]
public class GlobalClass(LevelClass level)
{
    public BasicList<Bead>? Solution;

    public int HowManyGuess
    {
        get
        {
            if (level.LevelChosen == 1)
            {
                return 4;
            }
            return level.LevelChosen + 4;
        }
    }
    public void StartCheckingSolution()
    {
        Solution!.ForEach(thisBead => thisBead.DidCheck = false);
    }
    internal BasicList<EnumColorPossibilities> ColorList { get; set; } = [];
}