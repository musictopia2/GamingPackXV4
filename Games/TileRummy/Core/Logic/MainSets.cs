namespace TileRummy.Core.Logic;
public class MainSets : MainSetsObservable<EnumColorType, EnumColorType, TileInfo, TileSet, SavedSet>
{
    public MainSets(CommandContainer command) : base(command)
    {

    }
    public void RemoveSet(int index)
    {
        var thisSet = SetList[index];
        RemoveSet(thisSet);
    }
    public void RedoSets()
    {
        SetList.ForEach(thisTemp =>
        {
            thisTemp.HandList.ForEach(thisTile =>
            {
                thisTile.IsUnknown = false; //to double check.
                thisTile.Drew = false;
                thisTile.IsSelected = false;
            });
        });
    }
    public bool PlayedAtLeastOneFromHand()
    {
        BasicList<int> tempList = new();
        SetList.ForEach(thisTemp =>
        {
            thisTemp.HandList.ForEach(thisTile =>
            {
                tempList.Add(thisTile.Deck);
            });
        });
        TileRummySaveInfo saveRoot = aa1.Resolver!.Resolve<TileRummySaveInfo>();
        foreach (var thisIndex in saveRoot.YourTiles)
        {
            if (tempList.Any(items => items == thisIndex))
            {
                return true;
            }
        }
        return false;
    }
}
