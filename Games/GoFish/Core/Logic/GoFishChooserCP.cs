namespace GoFish.Core.Logic;
public class GoFishChooserCP : SimpleEnumPickerVM<EnumRegularCardValueList>
{
    public void LoadFromHandCardValues(GoFishPlayerItem thisPlayer) //its smart enough to take their hand part
    {
        var thisList = thisPlayer.MainHandList.GroupBy(items => items.Value).Select(Items => Items.Key).ToBasicList();
        BasicList<BasicPickerData<EnumRegularCardValueList>> tempList = new();
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
    public GoFishChooserCP(CommandContainer command) : base(command, new CardValueListChooser()) { }
}