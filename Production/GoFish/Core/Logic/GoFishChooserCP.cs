namespace GoFish.Core.Logic;
public class GoFishChooserCP(CommandContainer command) : SimpleEnumPickerVM<EnumRegularCardValueList>(command, new CardValueListChooser())
{
    public void LoadFromHandCardValues(GoFishPlayerItem thisPlayer) //its smart enough to take their hand part
    {
        var thisList = thisPlayer.MainHandList.GroupBy(items => items.Value).Select(Items => Items.Key).ToBasicList();
        BasicList<BasicPickerData<EnumRegularCardValueList>> tempList = [];
        thisList.ForEach(items =>
        {
            BasicPickerData<EnumRegularCardValueList> thisPiece = new();
            thisPiece.EnumValue = items;
            thisPiece.IsEnabled = IsEnabled;
            thisPiece.IsSelected = false;
            tempList.Add(thisPiece);
        });
        ItemList.ReplaceRange(tempList);
    }
}