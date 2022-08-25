namespace Minesweeper.Core.Logic;
public static class Helpers
{
    public static async Task MessageGameOverAsync(this MinesweeperMainGameClass game, string message, IToast toast, ISystemError error)
    {
        toast.ShowInfoToast(message);
        await Task.Delay(2000); //wait 2 seconds so you can see the previous screen.
        await game.SendGameOverAsync(error);
    }
    public static void PopulateMinesNeeded(this ILevelVM level)
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