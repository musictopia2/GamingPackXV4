namespace GameLoaderBlazorLibrary;
public class GameRegistration
{
    public string GameName { get; set; } = "";
    public bool SupportsCompactLayout { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int NoPlayers { get; set; }
    public bool CanHaveExtraComputerPlayers { get; set; }
    public EnumPlayerType PlayerType { get; set; }

    public bool SupportsHumanGroup(int humanCount)
    {
        if (humanCount < 1 ||
            humanCount > MaxPlayers ||
            PlayerType == EnumPlayerType.SingleOnly)
        {
            return false;
        }

        if (CanHaveExtraComputerPlayers == false)
        {
            return humanCount >= MinPlayers &&
                humanCount != NoPlayers;
        }

        // Computers can fill any required remaining places.
        int totalPlayers = Math.Max(humanCount, MinPlayers);

        if (totalPlayers == NoPlayers)
        {
            totalPlayers++;
        }

        return totalPlayers <= MaxPlayers;
    }

}