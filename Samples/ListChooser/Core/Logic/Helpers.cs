namespace Minesweeper.Core.Logic;
public static class Helpers
{
    extension (ILevelVM level)
    {
        public void PopulateMinesNeeded()
        {
            if (level.LevelChosen.Value == EnumLevel.Easy.Value)
            {
                level.HowManyMinesNeeded = 10;
            }
            else if (level.LevelChosen.Value == EnumLevel.Medium.Value)
            {
                level.HowManyMinesNeeded = 20;
            }
            else
            {
                level.HowManyMinesNeeded = 30;
            }
        }
    }
}