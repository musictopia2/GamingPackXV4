namespace LifeBoardGame.Core.Logic;
public interface IStolenTileProcesses
{
    Task TilesStolenAsync(string player);
    Task ComputerStealTileAsync();
    void LoadOtherPlayerTiles();
}