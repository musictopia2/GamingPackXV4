namespace BasicGameFrameworkLibrary.Core.MahjongTileClasses;
[Cloneable(false)]
public class MahjongSolitaireTileInfo : BasicMahjongTile, IDeckObject, IMahjongTileInfo
{
    public void Populate(int chosen)
    {

        MahjongBasicTileHelper.PopulateTile(this, chosen);
    }
    public void Reset() { }
}