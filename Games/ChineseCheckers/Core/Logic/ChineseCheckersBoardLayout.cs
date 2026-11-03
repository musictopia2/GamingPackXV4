namespace ChineseCheckers.Core.Logic;
// Region numbers run clockwise from the bottom. Opposite regions are three apart.
public static class ChineseCheckersBoardLayout
{
    public static void ValidatePlayerCount(int count)
    {
        if (count != 2 && count != 3 && count != 4 && count != 6)
        {
            throw new CustomBasicException("Chinese Checkers requires 2, 3, 4, or 6 players. Five-player games are not allowed.");
        }
    }

    public static int OppositeRegion(int region) => ((region + 2) % 6) + 1;

    public static EnumColorChoice HomeColor(ChineseCheckersGameContainer container, int region)
    {
        if (region == 0)
        {
            return EnumColorChoice.None;
        }
        var players = container.PlayerList!.OrderBy(player => player.Id).ToArray();
        ValidatePlayerCount(players.Length);
        int[] regions = players.Length switch
        {
            2 => [1, 4],
            3 => [1, 3, 5],
            4 => [1, 2, 4, 5],
            6 => [1, 2, 3, 4, 5, 6],
            _ => []
        };
        for (int index = 0; index < regions.Length; index++)
        {
            if (regions[index] == region)
            {
                return players[index].Color;
            }
        }
        return EnumColorChoice.None;
    }
}
